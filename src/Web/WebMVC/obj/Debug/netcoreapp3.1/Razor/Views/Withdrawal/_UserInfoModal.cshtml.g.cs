#pragma checksum "C:\Users\User\Desktop\Richpay\src\Web\WebMVC\Views\Withdrawal\_UserInfoModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f8a494d195e2c2a33ff9bd7c2c570f4ed97a5a54"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Withdrawal__UserInfoModal), @"mvc.1.0.view", @"/Views/Withdrawal/_UserInfoModal.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f8a494d195e2c2a33ff9bd7c2c570f4ed97a5a54", @"/Views/Withdrawal/_UserInfoModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d07e873f05b36c9d83cd6a184d4bfbe1720fee4b", @"/Views/_ViewImports.cshtml")]
    public class Views_Withdrawal__UserInfoModal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<div id=""kt_modal_quick_view_user_info"" class=""modal fade"" role=""dialog"" aria-hidden=""true"" style=""z-index:99999"">
    <div class=""modal-dialog modal-dialog-centered"">
        <div class=""modal-content"">
            <div class=""modal-body modal-body-fit"">

                <!--begin: Tab Conetnt -->
                <div id=""modal_sub_datatable_ajax_source"">
                    <div class=""kt-portlet kt-portlet--tabs"">
                        <div class=""kt-portlet__head"">
                            <div class=""kt-portlet__head-label"">
                                <h3 class=""kt-portlet__head-title"">
                                    用户信息
                                    <!--<small>with lineawesome icons</small>-->
                                </h3>
                            </div>
                            <div class=""kt-portlet__head-toolbar"">
                                <ul class=""nav nav-tabs nav-tabs-line nav-tabs-line-brand nav-tabs-line-2x nav-tabs-line-right nav-tabs-b");
            WriteLiteral(@"old"" role=""tablist"">
                                    <li class=""nav-item"">
                                        <a class=""nav-link active"" data-toggle=""tab"" href=""#kt_modal_quick_view_user_info_tab_content_1"" role=""tab"">
                                            <i class=""fa fa-address-book"" aria-hidden=""true""></i>个人信息
                                        </a>
                                    </li>
                                    <li class=""nav-item"">
                                        <a class=""nav-link"" data-toggle=""tab"" href=""#kt_modal_quick_view_user_info_tab_content_2"" role=""tab"">
                                            <i class=""fa fa-check"" aria-hidden=""true""></i>帐户权限
                                        </a>
                                    </li>
                                    <li class=""nav-item"">
                                        <a class=""nav-link"" data-toggle=""tab"" href=""#kt_modal_quick_view_user_info_tab_content_3"" role=""tab"">
            ");
            WriteLiteral(@"                                <i class=""fa fa-money-check-alt"" aria-hidden=""true""></i>财务信息
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <!--begin:: Quick View/User Info-->
                        <div class=""kt-portlet__body"">
                            <div class=""tab-content"">
                                <div class=""kt-scroll tab-pane active"" id=""kt_modal_quick_view_user_info_tab_content_1"" role=""tabpanel"" data-scroll=""true"" data-height=""400"">
                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>名字</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=");
            WriteLiteral(@"""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                名称:
                                            </span>
                                            <span id=""quick_view_user_info_fullname"" class=""kt-widget13__text kt-widget13__text--bold"">
                                                Loop Inc.
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc kt-align-right"">
                                                昵称:
                                            </span>
                                            <span id=""quick_view_user_info_nickname"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                   ");
            WriteLiteral(@" </div>

                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>联络信息</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">

                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                手机号码:
                                            </span>
                                            <span id=""quick_view_user_info_phonenumber"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                              ");
            WriteLiteral(@"  电子邮件:
                                            </span>
                                            <span id=""quick_view_user_info_email"" class=""kt-widget13__text"">
                                                1,300
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                微信:
                                            </span>
                                            <span id=""quick_view_user_info_wechat"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                QQ:
                       ");
            WriteLiteral(@"                     </span>
                                            <span id=""quick_view_user_info_qq"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class=""kt-scroll tab-pane"" id=""kt_modal_quick_view_user_info_tab_content_2"" role=""tabpanel"" data-scroll=""true"" data-height=""400"">
                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>帐户信息</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc kt-align-right"">
                              ");
            WriteLiteral(@"                  用户名:
                                            </span>
                                            <span id=""quick_view_user_info_username"" class=""kt-widget13__text kt-widget13__text--bold"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                用户Id:
                                            </span>
                                            <span id=""quick_view_user_info_id"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                上级代理:
                                    ");
            WriteLiteral(@"        </span>
                                            <span id=""quick_view_user_info_upline"" class=""kt-widget13__text"">
                                            </span>
                                        </div>


                                    </div>

                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>帐户权限</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                添加代理:
                                            </span>
                                            <span id=""quick_view_user_info_hasgrantright"" class=""kt-widget13__text"">
                              ");
            WriteLiteral(@"              </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                帐户状态:
                                            </span>
                                            <span id=""quick_view_user_info_accountstatus"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                审核状态:
                                            </span>
                                            <span id=""quick_view_user_info_isreviewed"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
");
            WriteLiteral(@"                                    </div>

                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>帐户纪录</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                最后登录时间:
                                            </span>
                                            <span id=""quick_view_user_info_datelastloggedin"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
      ");
            WriteLiteral(@"                                          最后登录IP:
                                            </span>
                                            <span id=""quick_view_user_info_iplastloggedin"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                帐户建立时间:
                                            </span>
                                            <span id=""quick_view_user_info_datecreated"" class=""kt-widget13__text "">
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class=""kt-scroll tab-pane"" id=""kt_modal_quick_view_user_info_tab_content_3"" role=""tabpanel"" data-scr");
            WriteLiteral(@"oll=""true"" data-height=""400"">
                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>帐户余额</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                可用余额:
                                            </span>
                                            <span id=""quick_view_user_info_amountavailable"" class=""kt-widget13__text kt-widget13__text--bold"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
");
            WriteLiteral(@"                                                冻结余额:
                                            </span>
                                            <span id=""quick_view_user_info_amountforzen"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                总余额:
                                            </span>
                                            <span id=""quick_view_user_info_amounttotal"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                    </div>
                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>费率</span>
     ");
            WriteLiteral(@"                                   <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                支付宝费率:
                                            </span>
                                            <span id=""quick_view_user_info_ratealipay"" class=""kt-widget13__text"">
                                                Loop Inc.
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                微信费率:
                                            </span>
                                            <span id=""quick_view_user_inf");
            WriteLiteral(@"o_ratewechat"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                    </div>
                                    <div class=""kt-divider"">
                                        <span></span>
                                        <span>提现</span>
                                        <span></span>
                                    </div>
                                    <div class=""kt-widget13"">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                单笔限额:
                                            </span>
                                            <span id=""quick_view_user_info_eachamountlimit"" class=""kt-widget13__text kt-widget13__text--bold"">
                                            </span>
                                        </div");
            WriteLiteral(@">
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc kt-align-right"">
                                                单日限额:
                                            </span>
                                            <span id=""quick_view_user_info_dailyamountlimit"" class=""kt-widget13__text kt-widget13__text--bold"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                单日次数:
                                            </span>
                                            <span id=""quick_view_user_info_dailyfrequencylimit"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                 ");
            WriteLiteral(@"                       <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                提现手续费:
                                            </span>
                                            <span id=""quick_view_user_info_withdrawalcommission"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                        <div class=""kt-widget13__item"">
                                            <span class=""kt-widget13__desc"">
                                                入金手续费:
                                            </span>
                                            <span id=""quick_view_user_info_depositcommission"" class=""kt-widget13__text"">
                                            </span>
                                        </div>
                                    </div>
                            ");
            WriteLiteral(@"    </div>
                            </div>
                        </div>
                        <!--end:: Quick View/User Info-->
                    </div>
                </div>

                <!--end: Tab Content -->
            </div>
            <div class=""modal-footer kt-hidden"">
                <button type=""button"" class=""btn btn-clean btn-bold btn-upper btn-font-md"" data-dismiss=""modal"">Close</button>
                <button type=""button"" class=""btn btn-default btn-bold btn-upper btn-font-md"">Submit</button>
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
