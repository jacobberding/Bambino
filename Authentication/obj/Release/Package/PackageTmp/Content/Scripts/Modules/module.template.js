'use strict';

const Template = (function () {

    //Private ------------------------------------------------
    let _self = {
        vm: {},
        arr: [],
        name: `Template`
    };

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };

    const _init = (function () {

    })();

    return {
        getSelf: getSelf
    }

})();