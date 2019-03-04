'use strict';

const Template = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        pageArr: [],
        sort: `name asc`,
        isShowMore: false,
        name: `Template`,
        arr: [],
        vm: {},
        constructor: function (materialKey, name, isDeleted) {
            this.materialKey = materialKey;
            this.name = name;
            this.isDeleted = isDeleted;
        }
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