'use strict';

const ProjectContactCompany = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: [],
        vm: {},
        name: `ProjectContactCompany`,
        timeout: undefined
    };

    const _add = function () {

        const vm = {
            tableId: Project.getSelf().vm.projectId,
            manyKey: $(`#dboContactCompany${_self.name}`).val()
        };

        Global.post(`${_self.name}_Add`, vm)
            .done(function (data) {

                if ($(`m-card[data-key="${data.contactCompanyKey}"]`).length == 0)
                    $(`#flx${_self.name}Tags`).append(getHtmlTag(data));

                $(`#dboContactCompany${_self.name}`).val(``);

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _delete = function ($this) {

        const vm = {
            tableId: Project.getSelf().vm.projectId,
            manyKey: $this.attr(`data-key`)
        };

        Global.post(`${_self.name}_Delete`, vm)
            .done(function (data) {

                $(`m-card[data-key="${data.contactCompanyKey}"]`).remove();

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    //Public -----------------------------------------
    const get = function () {
        _get();
    };
    const getSelf = function () {
        return _self;
    };

    const getHtmlBody = function () {
        return `

            <m-flex data-type="col" class="w n" id="flx${_self.name}s">

                <m-flex data-type="row" class="n">

                    <h1 class="w">Contacts</h1>

                    <!--<m-flex data-type="row" class="w n">

                        <m-input class="n">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                    </m-flex>-->

                </m-flex>

                <m-flex data-type="col" class="w n pT" id="">
                    ${getHtmlSearch()}
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlTag = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule" data-function="ContactCompany.getHtmlModuleContacts" data-args="${obj.contactCompanyKey}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.isClient ? `Client` : ``} ${obj.isVendorDesign ? `Vendor Design` : ``} ${obj.isVendorIntegration ? `Vendor Integration` : ``}
                    </h2>
                </m-flex>
            </m-card>

            `;
    };
    const getHtmlSearch = function () {

        let html = ``;

        for (let projectContactCompany of Project.getSelf().vm.projectContactCompanies)
            html += getHtmlTag(projectContactCompany);

        return `

            ${Global.jack.mIM ? `
            <m-flex data-type="row" class="n s">

                <m-input class="">

                    <label for="dboContactCompany${_self.name}">Companies</label>

                    <m-flex data-type="row" class="n">
                        <select id="dboContactCompany${_self.name}">
                            <option value="">Select</option>
                            ${Global.getHtmlOptions(ContactCompany.getSelf().arr, [])}
                        </select>
                        <m-flex data-type="row" class="n c mL sm sQ secondary btnAdd${_self.name}">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>
                    </m-flex>

                </m-input>

            </m-flex>` : ``}

            <m-flex data-type="col" class="n s mB selectable" id="flx${_self.name}Tags">

                <m-flex data-type="row" class="tableRow n pL pR sC w">
                    <h2 class="">
                        Name
                    </h2>
                    <h2 class="">
                        Role
                    </h2>
                </m-flex>

                ${html}

            </m-flex>

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
        getHtmlBody: getHtmlBody,
        getHtmlTag: getHtmlTag,
        getHtmlSearch: getHtmlSearch
    };

})();