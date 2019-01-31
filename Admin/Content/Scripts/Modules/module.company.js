'use strict';

const Company = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: [],
        vm: {},
        name: `Company`,
        timeout: undefined
    }

    const _add = function () {

        const vm = {
            tableId: Members.getSelf().vm.memberId,
            name: $(`#txtSearch${_self.name}`).val()
        };

        Global.post(`Member_AddCompany`, vm)
            .done(function (data) {

                if ($(`m-card[data-id="${data.manyId}"]`).length == 0)
                    $(`#flxCompanyTags`).append(getHtmlTag(data));

                $(`#txtSearch${_self.name}`).val(``);

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _delete = function ($this) {

        const vm = {
            tableId: Members.getSelf().vm.memberId,
            manyId: $this.attr(`data-id`)
        };

        Global.post(`Member_DeleteCompany`, vm)
            .done(function (data) {

                $(`m-card[data-id="${vm.manyId}"]`).remove();

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _get = function () {

        Global.post(`${_self.name}_Get`, {})
            .done(function (data) {
                _self.arr = data;
            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _search = function (e, $this) {

        if (e.which == 13) {

            $(`m-select[data-name="${_self.name}"]`).remove();
            _add();

            return;

        }

        let options = ``;

        $(`m-select[data-name="${_self.name}"]`).remove();

        if ($(`#txtSearch${_self.name}`).val() == ``)
            return;

        for (let obj of _self.arr.filter(function (obj) { return obj.name.toLowerCase().includes($(`#txtSearch${_self.name}`).val().toLowerCase()); }))
            options += `<m-option data-name="${_self.name}">${obj.name}</m-option>`;

        $this.parent().append(`<m-select data-name="${_self.name}">${options}</m-select>`);

    }
    const _select = function ($this) {

        $(`#txtSearch${_self.name}`).val($this.html()).focus();
        $(`m-select[data-name="${_self.name}"]`).remove();

    }

    //Public -----------------------------------------
    const get = function () {
        _get();
    };
    const getSelf = function () {
        return _self;
    };

    const getHtmlTag = function (obj) {
        return `

            <m-card class="tag" data-id="${obj.companyId}">
                <m-flex data-type="row" class="n">
                    <h1 class="tE">
                        ${obj.name}
                    </h1>
                    <m-flex data-type="row" class="n c xs sQ tertiary btnDelete${_self.name}" data-id="${obj.companyId}">
                        <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                    </m-flex>
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `m-option[data-name="${_self.name}"]`, function () { _select($(this)); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function (e) { _search(e, $(this)); });
        $(document).on(`tap`, `.btnAdd${_self.name}`, function () { _add(); });
        $(document).on(`tap`, `.btnDelete${_self.name}`, function () { _delete($(this)); });
    })();

    return {
        get: get,
        getSelf: getSelf,
        getHtmlTag: getHtmlTag
    };

})();