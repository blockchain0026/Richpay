#pragma checksum "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\ManualCode\_UpdateQrCodeSettingsModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1fbce86579688e2d18f18037a81f4cdf5cb124c8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ManualCode__UpdateQrCodeSettingsModal), @"mvc.1.0.view", @"/Views/ManualCode/_UpdateQrCodeSettingsModal.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\_ViewImports.cshtml"
using WebMVC;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\_ViewImports.cshtml"
using WebMVC.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1fbce86579688e2d18f18037a81f4cdf5cb124c8", @"/Views/ManualCode/_UpdateQrCodeSettingsModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d07e873f05b36c9d83cd6a184d4bfbe1720fee4b", @"/Views/_ViewImports.cshtml")]
    public class Views_ManualCode__UpdateQrCodeSettingsModal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("form_update_qrcode_settings"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "ManualCode", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "UpdateQrCodeSettings", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("kt-form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<div id=""modal__update_qrcode_settings"" class=""modal fade"" tabindex=""-1"" role=""dialog"" aria-labelledby=""exampleModalLabel"" aria-hidden=""true"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"" id=""exampleModalLabel"">更新轮巡设定</h5>
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                </button>
            </div>
            <div class=""modal-body"">
                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1fbce86579688e2d18f18037a81f4cdf5cb124c85521", async() => {
                WriteLiteral(@"
                    <div class=""form-group"">
                        <label for=""userId"" class=""form-control-label"">二维码编号 :</label>
                        <input type=""text"" class=""form-control"" name=""qrcodeid"" readonly>
                    </div>
                    <div class=""col-xl-12"">
                        <div class=""kt-section__body"">
                            <div class=""form-group row"">
                                <div class=""col-lg-9 col-xl-6"">
                                    <h3 class=""kt-section__title kt-section__title-md"">轮巡配对</h3>
                                </div>
                            </div>
                        </div>
                        <div>
                            <label class=""font-weight-normal"">成功率</label>
                            <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""根据成功率自动关闭轮巡配对""></i>
                        </div>
                        <div class=""form-group row"">
          ");
                WriteLiteral(@"                  <div class=""col-lg-4"">

                                <div class=""input-group"">
                                    <span class=""kt-switch kt-switch--outline kt-switch--icon kt-switch--success"">
                                        <label>
                                            <input type=""checkbox"" name=""AutoPairingBySuccessRate"" data-val=""true"" value=""true"">
                                            <span></span>
                                        </label>
                                    </span>
                                </div>
                            </div>
                            <div class=""col-lg-4"">
                                <div class=""input-group"">
                                    <div class=""input-group-prepend"">
                                        <span class=""input-group-text"">
                                            <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""成功率低于此值时会");
                WriteLiteral("立刻关闭轮巡配对\"></i>\r\n                                        </span>\r\n                                    </div>\r\n                                    <input class=\"form-control text-center\"");
                BeginWriteAttribute("placeholder", " placeholder=\"", 2912, "\"", 2926, 0);
                EndWriteAttribute();
                WriteLiteral(@" value=""50"" name=""SuccessRateThresholdInHundredth"" disabled>
                                    <div class=""input-group-append""><span class=""input-group-text"">%</span></div>
                                </div>
                            </div>
                            <div class=""col-lg-4"">
                                <div class=""input-group"">
                                    <div class=""input-group-prepend"">
                                        <span class=""input-group-text"">
                                            <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""最低判断订单数""></i>
                                        </span>
                                    </div>
                                    <input type=""text"" class=""form-control text-center"" value=""5""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 3772, "\"", 3786, 0);
                EndWriteAttribute();
                WriteLiteral(@" name=""SuccessRateMinOrders"" disabled>
                                    <div class=""input-group-append""><span class=""input-group-text"">笔</span></div>
                                </div>
                            </div>
                        </div>
                        <div>
                            <label class=""font-weight-normal"">剩余额度</label>
                            <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""根据当日剩余额度自动关闭轮巡配对""></i>
                        </div>
                        <div class=""form-group row"">
                            <div class=""col-lg-4"">
                                <div class=""input-group kt-switch kt-switch--outline kt-switch--icon kt-switch--success"">
                                    <label>
                                        <input type=""checkbox"" name=""AutoPairingByQuotaLeft"" data-val=""true"" value=""true"">
                                        <span></span>
                             ");
                WriteLiteral(@"       </label>
                                </div>
                            </div>
                            <div class=""col-lg-8"">
                                <div class=""input-group"">
                                    <div class=""input-group-prepend"">
                                        <span class=""input-group-text"">
                                            阈值
                                        </span>
                                    </div>
                                    <input class=""form-control text-center""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 5375, "\"", 5389, 0);
                EndWriteAttribute();
                WriteLiteral(@" value=""50"" name=""QuotaLeftThreshold"" disabled>
                                    <div class=""input-group-append"">
                                        <span class=""input-group-text"">
                                            ¥
                                        </span>
                                    </div>
                                </div>
                                <span class=""form-text text-muted"">
                                    当日剩余额度<label class=""text-danger"">低于</label>此值时会立刻关闭轮巡配对
                                </span>
                            </div>
                        </div>

                        <div>
                            <label class=""font-weight-normal"">连续失败</label>
                            <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""根据连续失败次数自动关闭轮巡配对""></i>
                        </div>

                        <div class=""form-group row"">
                            <div class=""col-");
                WriteLiteral(@"lg-4"">
                                <div class=""input-group"">
                                    <span class=""kt-switch kt-switch--outline kt-switch--icon kt-switch--success"">
                                        <label>
                                            <input type=""checkbox"" name=""AutoPairingByCurrentConsecutiveFailures"" data-val=""true"" value=""true"">
                                            <span></span>
                                        </label>
                                    </span>
                                </div>
                            </div>
                            <div class=""col-lg-8"">
                                <div class=""input-group"">
                                    <div class=""input-group-prepend"">
                                        <span class=""input-group-text"">
                                            阈值
                                        </span>
                                    </div>
                     ");
                WriteLiteral("               <input class=\"form-control text-center\"");
                BeginWriteAttribute("placeholder", " placeholder=\"", 7492, "\"", 7506, 0);
                EndWriteAttribute();
                WriteLiteral(@" value=""50"" name=""CurrentConsecutiveFailuresThreshold"" disabled>
                                    <div class=""input-group-append"">
                                        <span class=""input-group-text"">
                                            次
                                        </span>
                                    </div>
                                </div>
                                <span class=""form-text text-muted"">二维码连续失败次数<label class=""text-danger"">高于</label>此值时会立刻关闭轮巡配对</span>
                            </div>
                        </div>


                        <div>
                            <label class=""font-weight-normal"">可用余额</label>
                            <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""根据交易员剩余的可用余额自动关闭轮巡配对""></i>
                        </div>
                        <div class=""form-group row"">
                            <div class=""col-lg-4"">
                                <div cla");
                WriteLiteral(@"ss=""input-group"">
                                    <span class=""kt-switch kt-switch--outline kt-switch--icon kt-switch--success"">
                                        <label>
                                            <input type=""checkbox"" name=""AutoPairngByAvailableBalance"" data-val=""true"" value=""true"">
                                            <span></span>
                                        </label>
                                    </span>
                                </div>
                            </div>
                            <div class=""col-lg-8"">
                                <div class=""input-group"">
                                    <div class=""input-group-prepend"">
                                        <span class=""input-group-text"">
                                            阈值
                                        </span>
                                    </div>
                                    <input class=""form-control text-center""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 9550, "\"", 9564, 0);
                EndWriteAttribute();
                WriteLiteral(@" value=""50"" name=""AvailableBalanceThreshold"" disabled>
                                    <div class=""input-group-append"">
                                        <span class=""input-group-text"">
                                            ¥
                                        </span>
                                    </div>
                                </div>
                                <span class=""form-text text-muted"">交易员可用余额<label class=""text-danger"">低于</label>此值时会立刻关闭轮巡配对</span>

                            </div>
                        </div>
                        <div class=""form-group row"">
                            <div class=""col-lg-6"">
                                <label>营业时间:</label>
                                <i class=""flaticon2-information"" data-toggle=""kt-tooltip"" data-placement=""right"" title=""根据平台营业时间自动关闭轮巡配对""></i>
                                <div class=""input-group"">
                                    <span class=""kt-switch kt-switch--outline kt");
                WriteLiteral(@"-switch--icon kt-switch--success"">
                                        <label>
                                            <input type=""checkbox"" name=""AutoPairingByBusinessHours"" data-val=""true"" value=""true"">
                                            <span></span>
                                        </label>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-secondary btn-md btn-tall btn-wide kt-font-bold kt-font-transform-u"" data-dismiss=""modal"">取消</button>
                <a href=""#"" data-action=""submit"" class=""btn btn-success btn-md btn-tall btn-wide kt-font-bold kt-font-transform-u"">提交</a>
            </div>
        </div>
    </div>
</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591