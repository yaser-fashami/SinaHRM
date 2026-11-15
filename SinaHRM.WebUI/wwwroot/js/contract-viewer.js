
(function ($, window, document) {
    "use strict";

    const ContractViewer = {

        // تنظیمات پیش‌فرض
        options: {
            apiUrl: "/api/contracts/{id}/html",

            contentSelector: "#contractContent",
            loadingSelector: "#contractLoading",
            errorSelector: "#contractError",
            printButtonSelector: "#btnPrint",
            closeButtonSelector: "#btnClose",

            signature: {
                employerTitle: "امضای کارفرما",
                employeeTitle: "امضای کارمند",
                employerName: "",
                employeeName: "",
                showNames: true
            },

            onLoaded: null,
            onError: null
        },

        // وضعیت داخلی
        state: {
            isLoading: false,
            currentContractId: null,
            currentHtml: null
        },

        /**
         * راه‌اندازی ماژول
         */
        init: function (options) {
            this.options = $.extend(
                true,
                {},
                this.options,
                options || {}
            );

            this.bindEvents();

            return this;
        },

        /**
         * اتصال رویدادها
         */
        bindEvents: function () {
            const self = this;

            $(document)
                .off("click.contractViewer", this.options.printButtonSelector)
                .on(
                    "click.contractViewer",
                    this.options.printButtonSelector,
                    function (event) {
                        event.preventDefault();
                        self.print();
                    }
                );

            $(document)
                .off("click.contractViewer", this.options.closeButtonSelector)
                .on(
                    "click.contractViewer",
                    this.options.closeButtonSelector,
                    function (event) {
                        event.preventDefault();
                        window.location.href = "/ContractManagement/HRContracts";
                    }
                );
        },

        /**
         * نمایش وضعیت بارگذاری
         */
        showLoading: function (show) {
            this.state.isLoading = !!show;

            $(this.options.loadingSelector).toggle(!!show);

            if (show) {
                $(this.options.errorSelector).hide().empty();
            }
        },

        /**
         * نمایش خطا
         */
        showError: function (message) {
            const errorMessage =
                message || "خطایی در پردازش قرارداد رخ داده است.";

            $(this.options.errorSelector)
                .text(errorMessage)
                .show();

            if (typeof this.options.onError === "function") {
                this.options.onError(errorMessage);
            }
        },

        /**
         * پاک کردن خطا
         */
        clearError: function () {
            $(this.options.errorSelector)
                .hide()
                .empty();
        },

        /**
         * پاک‌سازی HTML دریافتی
         *
         * نکته: HTML دریافتی از سرور همچنان باید
         * از نظر دسترسی و مجوز در سمت سرور کنترل شود.
         */
        sanitizeHtml: function (html) {
            if (typeof html !== "string" || !html.trim()) {
                throw new Error("محتوای قرارداد خالی است.");
            }

            if (!window.DOMPurify) {
                throw new Error(
                    "کتابخانه DOMPurify بارگذاری نشده است."
                );
            }

            return window.DOMPurify.sanitize(html, {
                ALLOWED_TAGS: [
                    "div", "span", "p", "br",
                    "h1", "h2", "h3", "h4", "h5", "h6",
                    "table", "thead", "tbody", "tfoot",
                    "tr", "td", "th",
                    "ul", "ol", "li",
                    "strong", "b", "em", "i", "u",
                    "small", "sup", "sub",
                    "img", "hr",
                    "section", "article",
                    "header", "footer",
                    "style"
                ],

                ALLOWED_ATTR: [
                    "class",
                    "style",
                    "dir",
                    "lang",
                    "colspan",
                    "rowspan",
                    "width",
                    "height",
                    "src",
                    "alt",
                    "title"
                ],

                FORBID_TAGS: [
                    "script",
                    "iframe",
                    "object",
                    "embed",
                    "form",
                    "input",
                    "button",
                    "textarea",
                    "select",
                    "link",
                    "meta",
                    "base"
                ],

                FORBID_ATTR: [
                    "onclick",
                    "onload",
                    "onerror",
                    "onmouseover",
                    "onfocus"
                ],

                ALLOW_DATA_ATTR: false
            });
        },

        /**
         * استخراج محتوای بدنه HTML
         *
         * API می‌تواند یک fragment یا یک سند کامل HTML برگرداند.
         */
        extractContractContent: function (html) {
            const parser = new DOMParser();

            const parsedDocument = parser.parseFromString(
                html,
                "text/html"
            );

            // حذف عناصر غیرمجاز یا غیرضروری
            $(parsedDocument).find(
                "script, iframe, object, embed, form, base"
            ).remove();

            return parsedDocument.body.innerHTML;
        },

        /**
         * ساخت HTML امضاها
         */

        buildSignatureHtml: function () {

            const signature = this.options.signature;

            const employerName = this.escapeHtml(
                signature.employerName || ""
            );

            const employeeName = this.escapeHtml(
                signature.employeeName || ""
            );

            return `
        <div class="contract-signatures">

            <div class="signature-wrapper">

                <div class="signature-box">

                    <div class="signature-title">
                        ${this.escapeHtml(signature.employerTitle)}
                    </div>

                    <div class="signature-line"></div>

                    ${signature.showNames
                    ? `
                            <div class="signature-name">
                                نام و نام خانوادگی: ${employerName}
                            </div>
                        `
                    : ""
                }

                </div>

                <div class="signature-box">

                    <div class="signature-title">
                        ${this.escapeHtml(signature.employeeTitle)}
                    </div>

                    <div class="signature-line"></div>

                    ${signature.showNames
                    ? `
                            <div class="signature-name">
                                نام و نام خانوادگی: ${employeeName}
                            </div>
                        `
                    : ""
                }

                </div>

            </div>

        </div>
    `;
        },

        /**
         * جلوگیری از تزریق HTML در اطلاعات امضا
         */
        escapeHtml: function (value) {
            return String(value ?? "").replace(
                /[&<>"']/g,
                function (character) {
                    const entities = {
                        "&": "&amp;",
                        "<": "&lt;",
                        ">": "&gt;",
                        '"': "&quot;",
                        "'": "&#039;"
                    };

                    return entities[character];
                }
            );
        },

        /**
         * نمایش قرارداد
         */
        showContract: function (html) {
            try {
                const cleanHtml = this.sanitizeHtml(html);

                const contractContent =
                    this.extractContractContent(cleanHtml);

                const $content = $(this.options.contentSelector);

                if (!$content.length) {
                    throw new Error(
                        "عنصر نمایش قرارداد در صفحه پیدا نشد."
                    );
                }

                // درج محتوای پاک‌سازی‌شده
                $content.html(contractContent);

                // جلوگیری از درج چندباره امضا
                // $content.find(".contract-signatures").remove();

                // // درج امضاها
                // $content.append(this.buildSignatureHtml());

                this.state.currentHtml = html;

                this.showLoading(false);
                this.clearError();

                if (typeof this.options.onLoaded === "function") {
                    this.options.onLoaded();
                }

                return true;

            } catch (error) {
                this.showLoading(false);
                this.showError(error.message);

                return false;
            }
        },

        /**
         * دریافت قرارداد از API
         */
        load: function (contractId) {
            const self = this;

            if (
                contractId === undefined ||
                contractId === null ||
                String(contractId).trim() === ""
            ) {
                this.showError("شناسه قرارداد معتبر نیست.");
                return;
            }

            if (this.state.isLoading) {
                return;
            }

            this.state.currentContractId = contractId;

            this.showLoading(true);

            const url = this.options.apiUrl.replace(
                "{id}",
                encodeURIComponent(contractId)
            );

            $.ajax({
                url: url,
                type: "GET",
                dataType: "html",
                cache: false,
                headers: {
                    "Accept": "text/html"
                }
            })

                .done(function (html) {
                    self.showContract(html);
                })

                .fail(function (xhr, status, error) {
                    self.showLoading(false);

                    let message =
                        "دریافت قرارداد با خطا مواجه شد.";

                    if (xhr.status === 401) {
                        message = "برای مشاهده قرارداد باید وارد سامانه شوید.";
                    } else if (xhr.status === 403) {
                        message = "شما مجوز مشاهده این قرارداد را ندارید.";
                    } else if (xhr.status === 404) {
                        message = "قرارداد موردنظر پیدا نشد.";
                    } else if (xhr.status >= 500) {
                        message = "خطای داخلی سرور رخ داده است.";
                    }

                    self.showError(message);

                    if (window.console) {
                        console.error(
                            "Contract API Error:",
                            status,
                            error
                        );
                    }
                })

                .always(function () {
                    self.state.isLoading = false;
                });
        },

        /**
         * چاپ قرارداد و ذخیره به صورت PDF
         */
        print: function () {

            const $content = $(this.options.contentSelector);

            if (!$content.length || !$content.children().length) {
                this.showError(
                    "ابتدا قرارداد را بارگذاری کنید."
                );
                return;
            }

            this.clearError();

            try {
                window.print();
            } catch (error) {
                this.showError(
                    "امکان چاپ قرارداد وجود ندارد."
                );
            }
        },

        /**
         * تغییر مشخصات امضا
         */
        // setSignature: function (signatureOptions) {
        //     this.options.signature = $.extend(
        //         {},
        //         this.options.signature,
        //         signatureOptions || {}
        //     );

        //     const $content = $(this.options.contentSelector);

        //     if ($content.length && this.state.currentHtml) {
        //         $content.find(".contract-signatures").remove();

        //         $content.append(this.buildSignatureHtml());
        //     }

        //     return this;
        // },

        /**
         * پاک کردن قرارداد
         */
        clear: function () {
            $(this.options.contentSelector).empty();

            this.state.currentHtml = null;
            this.state.currentContractId = null;

            this.clearError();
            this.showLoading(false);

            return this;
        }
    };

    // دسترسی عمومی
    window.ContractViewer = ContractViewer;

})(jQuery, window, document);

