Param(
    [parameter(Mandatory=$false)][string]$registry,
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][string]$execPath,
    [parameter(Mandatory=$false)][string]$kubeconfigPath,
    [parameter(Mandatory=$true)][string]$configFile,
    [parameter(Mandatory=$false)][string]$imageTag="dev",
    [parameter(Mandatory=$false)][bool]$deployCI=$false,
    [parameter(Mandatory=$false)][bool]$buildImages=$true,
    [parameter(Mandatory=$false)][bool]$pushImages=$true,
    [parameter(Mandatory=$false)][bool]$deployInfrastructure=$true,
    [parameter(Mandatory=$false)][string]$dockerOrg="blockchain0026"
)

function ExecKube($cmd) {    
    if($deployCI) {
        $kubeconfig = $kubeconfigPath + 'config';
        $exp = $execPath + 'kubectl ' + $cmd + ' --kubeconfig=' + $kubeconfig
        Invoke-Expression $exp
    }
    else{
        $exp = $execPath + 'kubectl ' + $cmd
        Invoke-Expression $exp
    }
}

# Initialization
$debugMode = $PSCmdlet.MyInvocation.BoundParameters["Debug"].IsPresent
$useDockerHub = [string]::IsNullOrEmpty($registry)

$externalDns = & ExecKube -cmd 'get svc ingress-nginx -n ingress-nginx -o=jsonpath="{.status.loadBalancer.ingress[0].ip}"'
Write-Host "Ingress ip detected: $externalDns" -ForegroundColor Yellow 

if (-not [bool]($externalDns -as [ipaddress])) {
    Write-Host "Must install ingress first" -ForegroundColor Red
    Write-Host "Run deploy-ingress.ps1 and  deploy-ingress-azure.ps1" -ForegroundColor Red
    exit
}


# Check required commands (only if not in CI environment)
if(-not $deployCI) {
        $requiredCommands = ("docker", "docker-compose", "kubectl")
        foreach ($command in $requiredCommands) {
        if ((Get-Command $command -ErrorAction SilentlyContinue) -eq $null) {
            Write-Host "$command must be on path" -ForegroundColor Red
            exit
        }
    }
}
else {
    $buildImages = false;       # Never build images through CI, as they previously built
}

# Get tag to use from current branch if no tag is passed
if ([string]::IsNullOrEmpty($imageTag)) {
    $imageTag = $(git rev-parse --abbrev-ref HEAD)
}
Write-Host "Docker image Tag: $imageTag" -ForegroundColor Yellow

# building  docker images if needed
if ($buildImages) {
    Write-Host "Building Docker images tagged with '$imageTag'" -ForegroundColor Yellow
    $env:TAG=$imageTag
    docker-compose -p .. -f ../docker-compose.yml build    
}

if ($pushImages) {
    Write-Host "Pushing images to $registry/$dockerOrg..." -ForegroundColor Yellow
    $services = ("identity.api", "whiskyrecording.api", "webmvc",  "ocelotapigw",  "webwhiskyarchiveagg")

    foreach ($service in $services) {
        $imageFqdn = if ($useDockerHub)  {"$dockerOrg/${service}"} else {"$registry/$dockerOrg/${service}"}
        docker tag whiskyarchive/${service}:$imageTag ${imageFqdn}:$imageTag
        docker push ${imageFqdn}:$imageTag            
    }
}

# if we have login/pwd add the secret to k8s
if (-not [string]::IsNullOrEmpty($dockerUser)) {
    $registryFDQN =  if (-not $useDockerHub) {$registry} else {"index.docker.io/v1/"}

    Write-Host "Logging in to $registryFDQN as user $dockerUser" -ForegroundColor Yellow
    if ($useDockerHub) {
        docker login -u $dockerUser -p $dockerPassword
    }
    else {
        docker login -u $dockerUser -p $dockerPassword $registryFDQN
    }
    
    if (-not $LastExitCode -eq 0) {
        Write-Host "Login failed" -ForegroundColor Red
        exit
    }

    # Try to delete the Docker registry key secret
    ExecKube -cmd 'delete secret docker-registry registry-key'

    # Create the Docker registry key secret
    ExecKube -cmd 'create secret docker-registry registry-key `
    --docker-server=$registryFDQN `
    --docker-username=$dockerUser `
    --docker-password=$dockerPassword `
    --docker-email=not@used.com'
}

# Removing previous services & deployments
Write-Host "Removing existing services & deployments.." -ForegroundColor Yellow
ExecKube -cmd 'delete deployments --all'
ExecKube -cmd 'delete services --all'
ExecKube -cmd 'delete configmap internalurls'
ExecKube -cmd 'delete configmap urls'
ExecKube -cmd 'delete configmap externalcfg'
ExecKube -cmd 'delete configmap ocelot'
ExecKube -cmd 'delete -f ingress.yaml'

# start sql, rabbitmq, frontend deployments
if ($deployInfrastructure) {
    Write-Host 'Deploying infrastructure deployments (databases, redis, RabbitMQ...)' -ForegroundColor Yellow
    ExecKube -cmd 'create -f sql-data.yaml -f rabbitmq.yaml'
}


Write-Host 'Deploying ocelot APIGW' -ForegroundColor Yellow

ExecKube "create configmap ocelot --from-file=ww=ocelot/configuration-web-whiskyarchive.json"
ExecKube -cmd "apply -f ocelot/deployment.yaml"
ExecKube -cmd "apply -f ocelot/service.yaml"

Write-Host 'Deploying code deployments (Web APIs, Web apps, ...)' -ForegroundColor Yellow
ExecKube -cmd 'create -f services.yaml'

ExecKube -cmd 'create -f internalurls.yaml'
ExecKube -cmd 'create configmap urls `
    --from-literal=mvc_e=http://$($externalDns)/webmvc `
    --from-literal=webwhiskyarchiveapigw_e=http://$($externalDns)/webwhiskyarchiveapigw `
    --from-literal=webwhiskyarchiveagg_e=http://$($externalDns)/webwhiskyarchiveagg `
    --from-literal=identity_e=http://$($externalDns)/identity `
    --from-literal=whiskyrecording_e=http://$($externalDns)/whiskyrecording-api'

ExecKube -cmd 'label configmap urls app=whiskyarchive'


ExecKube -cmd 'create configmap cfg-whiskyarchive-identity-api `
    --from-literal=mvc_e=http://$($externalDns)/webmvc'

ExecKube -cmd 'label configmap cfg-whiskyarchive-identity-api app=whiskyarchive'


ExecKube -cmd 'create configmap cfg-whiskyarchive-webmvc `
    --from-literal=urls__mvc=http://$($externalDns)/webmvc `
    --from-literal=urls__IdentityUrl=http://$($externalDns)/identity'

ExecKube -cmd 'label configmap cfg-whiskyarchive-webmvc app=whiskyarchive'


Write-Host "Deploying configuration from $configFile" -ForegroundColor Yellow
ExecKube -cmd "create -f $configFile"

Write-Host "Creating deployments..." -ForegroundColor Yellow
ExecKube -cmd 'create -f deployments.yaml'

# update deployments with the correct image (with tag and/or registry)
$registryPath = ""
if (-not [string]::IsNullOrEmpty($registry)) {
    $registryPath = "$registry/"
}

Write-Host "Update Image containers to use prefix '$registry$dockerOrg' and tag '$imageTag'" -ForegroundColor Yellow

ExecKube -cmd 'set image deployments/identity identity=${registryPath}${dockerOrg}/identity.api:$imageTag'
ExecKube -cmd 'set image deployments/whiskyrecording whiskyrecording=${registryPath}${dockerOrg}/whiskyrecording.api:$imageTag'
ExecKube -cmd 'set image deployments/webmvc webmvc=${registryPath}${dockerOrg}/webmvc:$imageTag'

ExecKube -cmd 'set image deployments/webwhiskyarchiveagg webwhiskyarchiveagg=${registryPath}${dockerOrg}/webwhiskyarchiveagg:$imageTag'

ExecKube -cmd 'set image deployments/apigwww apigwww=${registryPath}${dockerOrg}/ocelotapigw:$imageTag'

Write-Host "Execute rollout..." -ForegroundColor Yellow
ExecKube -cmd 'rollout resume deployments/identity'
ExecKube -cmd 'rollout resume deployments/whiskyrecording'
ExecKube -cmd 'rollout resume deployments/webmvc'
ExecKube -cmd 'rollout resume deployments/webwhiskyarchiveagg'
ExecKube -cmd 'rollout resume deployments/apigwww'

Write-Host "Adding/Updating ingress resource..." -ForegroundColor Yellow
ExecKube -cmd 'apply -f ingress.yaml'

Write-Host "WebSPA is exposed at http://$externalDns, WebMVC at http://$externalDns/webmvc, WebStatus at http://$externalDns/webstatus" -ForegroundColor Yellow

