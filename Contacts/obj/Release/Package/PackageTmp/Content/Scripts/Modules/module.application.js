'use strict';

const Application = (function () {

    //Private -----------------------------------------------------
    const _open = function () {

        $(`body`).append(Application.getHtmlBody());

    }

    //Public -----------------------------------------------------

    const getHtmlBody = function () {
        return `

            <m-body data-label="Primary">    
                ${Contacts.getHtmlBody()}
            </m-body>

            `;
    }

    const _init = (function () {
        _open();
    })();

    return {
        getHtmlBody: getHtmlBody
    }

})();
