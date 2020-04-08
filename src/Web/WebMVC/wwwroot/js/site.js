function ReturnPreviousPage() {
    window.history.back();
}

function convertUTCDateToLocalDate(date) {
    var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);

    var offset = date.getTimezoneOffset() / 60;
    var hours = date.getHours();

    newDate.setHours(hours - offset);

    return newDate;
}
function convertLocalDateToUtcDate(date) {
    var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);
    console.log(newDate);
    var offset = date.getTimezoneOffset() / 60;
    var hours = date.getHours();

    newDate.setHours(hours + offset);
    return newDate;
}



function convertLocalHoursToUTCHours(hours, minutes) {
    var date = new Date();
    date.setHours(hours);
    date.setMinutes(minutes);

    var utcDate = convertLocalDateToUtcDate(date);
    var utcHours = utcDate.getHours();
    return utcHours;
}

function convertLocalMinsToUTCMins(hours, minutes) {
    var date = new Date();
    date.setHours(hours);
    date.setMinutes(minutes);

    var utcDate = convertLocalDateToUtcDate(date);
    var utcMins = utcDate.getMinutes();
    return utcMins;
}


function convertUTCHoursToLocalHours(hours, minutes) {
    var date = new Date();
    date.setHours(hours);
    date.setMinutes(minutes);

    var localDate = convertUTCDateToLocalDate(date);
    var localHours = localDate.getHours();
    return localHours;
}

function convertUTCMinsToLocalMins(hours, minutes) {
    var date = new Date();
    date.setHours(hours);
    date.setMinutes(minutes);

    var localDate = convertUTCDateToLocalDate(date);
    var localMins = localDate.getMinutes();
    return localMins;
}




function getTimezoneDifferenceInMinutes() {
    var date = new Date();
    var mins = date.getTimezoneOffset();

    return mins;
}