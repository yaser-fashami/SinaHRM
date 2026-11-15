
(function (window, document, $) {
    'use strict';

    if (!$) {
        throw new Error('PageLoader: jQuery must load first.');
    }

    if (window.PageLoader) {
        return;
    }

    const config = {
        text: 'لطفاً صبر کنید...',
        ajax: true,
        delay: 150
    };

    let loader = null;
    let message = null;
    let manualVisible = false;
    let pendingRequests = 0;
    let showTimer = null;

    function createLoader() {
        if (loader && document.body.contains(loader)) {
            return;
        }
        
        loader = document.createElement('div');
        loader.id = 'app-page-loader';
        loader.className = 'page-loader flex-column bg-dark bg-opacity-25';
        loader.setAttribute('role', 'status');
        loader.setAttribute('aria-live', 'polite');
        loader.setAttribute('aria-hidden', 'true');

        const spinner = document.createElement('span');
        spinner.className = 'spinner-border text-primary';
        spinner.setAttribute('aria-hidden', 'true');

        message = document.createElement('span');
        message.className = 'text-gray-800 fs-6 fw-semibold mt-5';
        message.textContent = config.text;

        loader.append(spinner, message);
        document.body.appendChild(loader);
    }

    function isVisible() {
        return manualVisible ||
            (config.ajax && pendingRequests > 0);
    }

    function render() {
        createLoader();
        
        const visible = isVisible();

        loader.classList.toggle('is-visible', visible);
        loader.setAttribute('aria-hidden', String(!visible));
    }

    function clearShowTimer() {
        if (showTimer !== null) {
            window.clearTimeout(showTimer);
            showTimer = null;
        }
    }

    const api = {
        init: function (options) {
            Object.assign(config, options || {});

            createLoader();
            message.textContent = config.text;

            if (config.ajax) {
                this.bindAjax();
            } else {
                $(document).off('.appPageLoader');
                pendingRequests = 0;
                clearShowTimer();
            }

            render();
            return this;
        },

        show: function (text) {
            clearShowTimer();

            if (typeof text === 'string') {
                this.setText(text);
            }

            manualVisible = true;
            render();

            return this;
        },

        hide: function () {
            manualVisible = false;
            render();

            return this;
        },

        setText: function (text) {
            createLoader();
            config.text = String(text);
            message.textContent = config.text;

            return this;
        },

        bindAjax: function () {
            $(document)
                .off('.appPageLoader')
                .on('ajaxSend.appPageLoader', function () {
                    pendingRequests++;

                    clearShowTimer();

                    showTimer = window.setTimeout(function () {
                        showTimer = null;

                        if (config.ajax && pendingRequests > 0) {
                            render();
                        }
                    }, Math.max(0, Number(config.delay) || 0));
                })
                .on('ajaxComplete.appPageLoader', function () {
                    pendingRequests = Math.max(
                        0,
                        pendingRequests - 1
                    );

                    if (pendingRequests === 0) {
                        clearShowTimer();
                        render();
                    }
                });

            return this;
        }
    };

    window.PageLoader = api;

    function initialize() {
        createLoader();
        render();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize, {
            once: true
        });
    } else {
        initialize();
    }

})(window, document, window.jQuery);
