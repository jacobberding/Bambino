'use strict';

const Company = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: [],
        vm: {},
        name: `Company`
    }

    const _get = function () {
        
        Global.post(`${_self.name}_Get`, {})
            .done(function (data) {
                _self.arr = data;
            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    //Public -----------------------------------------
    const get = function () {
        _get();
    };
    const getSelf = function () {
        return _self;
    };

    const _init = function () {

    };

    return {
        get: get,
        getSelf: getSelf
    };

})();