#pragma checksum "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "bcee06506d36d7257d01ef72eba01abd7a6d09de"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Pay_Index), @"mvc.1.0.view", @"/Views/Pay/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bcee06506d36d7257d01ef72eba01abd7a6d09de", @"/Views/Pay/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d07e873f05b36c9d83cd6a184d4bfbe1720fee4b", @"/Views/_ViewImports.cshtml")]
    public class Views_Pay_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebMVC.ViewModels.PayViewModels.IndexViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/pay.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("media", new global::Microsoft.AspNetCore.Html.HtmlString("screen"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("text/javascript"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/jquery.qrcode.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/layui.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/clipboard.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("layuicss-layer"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_9 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/layer.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_10 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("media", new global::Microsoft.AspNetCore.Html.HtmlString("all"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_11 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/box_top.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_12 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/box_middle.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_13 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/alipay_qr_scan.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_14 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("alt", new global::Microsoft.AspNetCore.Html.HtmlString(""), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_15 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("style", new global::Microsoft.AspNetCore.Html.HtmlString("width: 40px;height:40px;float: right;margin-right: 18px;margin-top: 8px;"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_16 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/pay2/box_bottom.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_17 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("qrcode_page"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<!DOCTYPE html>\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "bcee06506d36d7257d01ef72eba01abd7a6d09de10030", async() => {
                WriteLiteral("\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n\r\n    <meta name=\"viewport\" content=\"width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0\">\r\n    <title>截图到相册 - 扫码付款</title>\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "bcee06506d36d7257d01ef72eba01abd7a6d09de10549", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "bcee06506d36d7257d01ef72eba01abd7a6d09de11816", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "bcee06506d36d7257d01ef72eba01abd7a6d09de13003", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "bcee06506d36d7257d01ef72eba01abd7a6d09de14190", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_6);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "bcee06506d36d7257d01ef72eba01abd7a6d09de15377", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_7);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "bcee06506d36d7257d01ef72eba01abd7a6d09de16564", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_8);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_9);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_10);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "bcee06506d36d7257d01ef72eba01abd7a6d09de18623", async() => {
                WriteLiteral("\r\n    <div class=\"container\">\r\n        <div class=\"logo\">付款成功后，5分钟内未到账，请立即联系客服！</div>\r\n        <div class=\"box_top\">");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "bcee06506d36d7257d01ef72eba01abd7a6d09de19006", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_11);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("</div>\r\n        <div class=\"box\">\r\n            <h1>￥<span>");
#nullable restore
#line 25 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
                  Write(Model.Amount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</span></h1>\r\n            <h2 id=\"orderSN_data\">\r\n                <span class=\"orderSN_data_No\">\r\n                    订单号：\r\n                    <span>");
#nullable restore
#line 29 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
                     Write(Model.OrderTrackingNumber);

#line default
#line hidden
#nullable disable
                WriteLiteral("</span>\r\n                </span>\r\n                <span class=\"orderSN_data_copy\" data-clipboard-text=\"");
#nullable restore
#line 31 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
                                                                Write(Model.OrderTrackingNumber);

#line default
#line hidden
#nullable disable
                WriteLiteral("\">复制</span>\r\n            </h2>\r\n        </div>\r\n        <div>");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "bcee06506d36d7257d01ef72eba01abd7a6d09de21242", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_12);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"</div>
        <div class=""box h300"" style=""padding:1px 0"">
            <div class=""tips"" style=""margin-bottom:10px"">
                <p style=""margin: 0px;font-size:15px;font-weight:bold;color: #01a9f4"">
                    截图保存相册 →
                    <span>打开支付宝</span>
                    → 识别二维码付款
                </p>
            </div>
            <div class=""qrcode_box"" style=""position:relative"">
                <div id=""qrcode"" style=""display: block; width: 210px; height: 210px;background:url(/static/images/qrcode_loading.png) center top;"">
                    <img");
                BeginWriteAttribute("src", " src=\"", 2069, "\"", 2097, 1);
#nullable restore
#line 45 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
WriteAttributeValue("", 2075, Model.QrCodeImageData, 2075, 22, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                BeginWriteAttribute("alt", " alt=\"", 2098, "\"", 2104, 0);
                EndWriteAttribute();
                WriteLiteral(@" style=""height:210px;width:210px"" />
                </div>

                <div id=""loading"" style=""display: none;"">
                    <span class=""loading""></span>
                    <span class=""load-text"">正在载入二维码...</span>
                </div>

            </div>
            <span class=""tip"">请在下面时间内完成付款</span>
            <div class=""time"">
                <strong id=""minute_show"" style=""background:#01a9f4""><s></s>04</strong><span class=""time_dot"" style=""color:#01a9f4"">:</span><strong id=""second_show"" style=""background:#01a9f4""><s></s>32</strong>
            </div>
            <div class=""ico-scan_alipay_qr""></div>
            <div id=""tipText"" style=""width: 100%;height:56px;margin-top: 15px;"">
                <div style=""width: 45%;height:56px;float: left;"">
                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "bcee06506d36d7257d01ef72eba01abd7a6d09de24273", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_13);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_14);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_15);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                </div>
                <div id=""showtext"" style=""line-height: 56px;height: 56px; width: 48%;float: left;text-align: left;padding-left: 2%;font-size: 18px;"">
                    <span style=""width: 100px;height:40px;line-height:40px;margin-top:8px;border-radius: 5px;color: white;background:#01a9f4;display:block;text-align:center"">打开支付宝</span>
                </div>
            </div>

            <div id=""leftCaption"">
                <span class=""leftCaption1"">
                    请按提单金额付款
                </span>
                <span class=""leftCaption2"">
                    ，否则不到账
                </span>
            </div>
            <div id=""rightCaption"">
                <span class=""rightCaption1"">
                    请勿重复扫码付款
                </span>
                <span class=""rightCaption2"">
                    ，否则不到账
                </span>
            </div>
        </div>
        <div>");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "bcee06506d36d7257d01ef72eba01abd7a6d09de26504", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_16);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"</div>
    </div>
    <script>
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.alert('温馨提示', {
                title: ""注意事项"",
                content: '<div>\
                                                        <p><span style=""font-size:16px;color:red;font-weight:bold;"">1.不要重复扫码付款</span>，否则不到账</p>\
                                                        <p><span style=""font-size:16px;color:red;font-weight:bold;"">2.付款金额与订单金额一致</span>，否则不到账</p>\
                                                        <p><span style=""font-size:16px;color:red;font-weight:bold;"">3.付款成功后5分钟未到账</span>，请联系客服处理</p>\
                                                    </div>'
            });
        });

        $('#showtext').click(function () {
            window.location = '");
#nullable restore
#line 101 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
                          Write(Model.PaymentCommand);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"';
        });

        var alicode_timer;
        var check_order_timer;
        var myTimer;
        var count = 3;

        jQuery(function () {
            var clipboard = new ClipboardJS('.orderSN_data_copy');
            clipboard.on('success', function (e) {
                layui.use('layer', function () {
                    var layer = layui.layer;
                    layer.msg(""复制成功"");
                });
                e.clearSelection();
            });

            check_order_timer = setInterval(function () {
                check_pay_result();
            }, 5000);
            //get_qrcode();
        })

        function get_qrcode() {

            /*jQuery('#qrcode').qrcode({
                width: 210,
                height: 210,
                text: data.qrcode
            });*/

            $.ajax({
                type:""POST"",
                dataType:""JSON"",
                url:""/getqrcode"",
                data:JSON.stringify({ ""order_sn"" : ""1071578");
                WriteLiteral(@"93030105804605080""}),
                success: function(data){

                        if (data.code == 1){

                            $(""#loading"").css(""display"",""none"");

                            $(""#qrcode"").html('');

                            jQuery('#qrcode').qrcode({
                                width: 210,
                                height: 210,
                                text: data.qrcode
                            });

                            alicode_timer = window.setInterval(function(){
                                check_pay_result();
                            },2000)

                        }else if(data.code == 10){

                            if(count <= 0){

                                 $(""#loading"").hide();
                                 $(""#qrcode"").html('');
                                 $(""#qrcode"").css(""background-image"",""url('/static/images/msg.png')"");
                                 $(""#qrcode"").css(""background-size"",");
                WriteLiteral(@"""100%"");

                                 $('.tip').html('<b class=""red"">二维码匹配失败，请稍后再试</b>');
                            }else{
                                setTimeout(function(){
                                    get_qrcode();
                                    count--;
                                },2000)
                            }

                        }else {

                             setTimeout(function(){
                                get_qrcode();
                             },2000)

                        }

                }
            });
        }

        function timer(intDiff) {

            myTimer = window.setInterval(function () {

                var day = 0,
                    hour = 0,
                    minute = 0,
                    second = 0;
                if (intDiff > 0) {
                    day = Math.floor(intDiff / (60 * 60 * 24));
                    hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
             ");
                WriteLiteral(@"       minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
                    second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
                }

                if (minute <= 9) minute = '0' + minute;
                if (second <= 9) second = '0' + second;

                $('#minute_show').html('<s></s>' + minute);
                $('#second_show').html('<s></s>' + second);

                if (hour <= 0 && minute <= 0 && second <= 0) {

                    qrcode_timeout();
                    clearInterval(myTimer);
                    window.clearInterval(alicode_timer);
                }
                intDiff--;


            }, 1000);
        }

        timer(");
#nullable restore
#line 218 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
         Write(Model.TimeLeft);

#line default
#line hidden
#nullable disable
                WriteLiteral(@");


        function qrcode_timeout() {
            $(""#qrcode"").html('');
            $(""#qrcode"").css(""background-image"", ""url('/static/images/qrcode_timeout.png')"");

            $('.tip').html('<b class=""red"">二维码已失效</b>');
            $(""#loading"").hide();
        }

        function check_pay_result() {
            $.ajax({
                url: ""Pay/CheckOrderStatus"",
                type: ""Get"",
                contentType: ""application/json"",
                data: { orderTrackingNumber: '");
#nullable restore
#line 234 "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Pay\Index.cshtml"
                                         Write(Model.OrderTrackingNumber);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"' },
                success: function (data) {

                    if (data.status == 0) {
                    window.clearInterval(myTimer);
                    window.clearInterval(alicode_timer);
                    window.clearInterval(check_order_timer);


                    $(""#qrcode"").html('');
                    $(""#qrcode"").css(""background-image"", ""url('/static/images/pay_ok.png')"");


                    $('#minute_show').html('<s>00</s>');
                    $('#second_show').html('<s>00</s>');

                    var i = 10;

                        var jump_timer = setInterval(function () {
                            $("".tip"").html('<b class=""red"">' + i + '</b>秒后跳转至商户页');
                            i--;
                            if (i < 0) {
                                clearInterval(jump_timer);

                                $(""#msg"").html('正在跳转至商户页...');

                                if (data.return_url) {
                                    window");
                WriteLiteral(@".location = data.return_url;
                                }
                            }
                        }, 1000);
                }
			},
			error: function (response) {
				console.log(response.responseText);
            }
            })
        }
    </script>

");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_17);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebMVC.ViewModels.PayViewModels.IndexViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
