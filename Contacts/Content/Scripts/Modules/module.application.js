'use strict';

const Application = (function () {

    //Private -----------------------------------------------------
    const _open = function () {

        $(`body`).append(_getHtmlBody());

    }

    const _getHtmlBody = function () {
        return `

            <m-body data-label="Primary">    
                ${Contacts.getHtmlBody()}
            </m-body>

            `;
    }

    //Public -----------------------------------------------------

    const _init = (function () {
        _open();
    })();

    return {

    }

})();
