'use strict';

const Project = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 5,
        page: 1,
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        arr: [],
        name: `Project`
    };

    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort
        }

        if (page == 1) _self.arr = [];

        $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        _self.page = page;
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.arr.push(obj);

                $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
                $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(getHtmlBodyList(_self.arr));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    }
    const _getById = function (id) {

        const vm = {
            id: id
        }

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`#flxBodyDetail${_self.name}`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    };
    const _sort = function ($this) {

        var dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    };

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };
    const getByPage = function (page) {
        _getByPage(page);
    };

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" class="w" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}s</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        <!--<m-flex data-type="row" class="n c sm sQ primary btnOpenModule" data-function="Materials.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>-->

                    </m-flex>

                </m-flex>
            
                ${Global.getHtmlLoading()}

            </m-flex>

            `;
    }
    const getHtmlBodyList = function (arr) {

        let html = ``;

        for (let obj of arr)
            html += getHtmlCard(obj);

        return `
            <m-flex data-type="col" class="s cards selectable" id="lst${_self.name}s">

                <m-flex data-type="row" class="tableRow n pL pR mB sC sort" style="width: 100%;">
                    <h2 class="${Global.getSort(_self.sort, `name`)}" data-sort="name">
                        Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `code`)}" data-sort="code">
                        Code
                    </h2>
                </m-flex>

                ${html}

                ${(_self.isShowMore) ? `
                <m-card class="load" id="btnShowMore${_self.name}">
                    <m-flex data-type="row" class="c">
                        <h2>
                            Show More
                        </h2>
                    </m-flex>
                </m-card>` : ``}

            </m-flex>
            `;
    }
    const getHtmlBodyById = function (id) {
        _getById(id);
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                ${Global.jack.mIA ? `
                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="project,btnDelete${_self.name}">
                    <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                </m-flex>` : ``}
            
            </m-flex>

            <m-flex data-type="col" class="" id="flxBodyDetail${_self.name}">
                <h1 class="loading">Loading</h1>
            </m-flex>

            `;
    }
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="s n">

                <m-flex data-type="col" class="w n">
                    ${_self.vm.name}
                </m-flex>

                ${ProjectMember.getHtmlBody()}

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenBody mB" data-label="Primary" data-function="Project.getHtmlBodyById" data-args="${obj.projectId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.code}
                    </h2>
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`tap`, `#btnAdd${_self.name}, #btnEdit${_self.name}`, function () { _addEdit(); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete(); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); });
    })();

    return {
        getSelf: getSelf,
        getByPage: getByPage,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyById: getHtmlBodyById,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlCard: getHtmlCard
    }

})();
const ProjectMember = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 10,
        page: 1,
        sort: `email asc`,
        isShowMore: false,
        vm: {},
        arr: [],
        name: `ProjectMember`
    };

    const _add = function () {
        
        const vm = {
            projectId: Project.getSelf().vm.projectId,
            email: $(`#txtSearch${_self.name}`).val()
        };

        Global.post(`Project_AddMember`, vm)
            .done(function (data) {

                $(`#flx${_self.name}s`).append(getHtmlCard(data));
                $(`#txtSearch${_self.name}`).val(``);

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(1, `Error`, data.responseJSON.Message, `error`);
            });

    };

    const _delete = function ($this) {

        const vm = {
            projectId: Project.getSelf().vm.projectId,
            token: $this.attr(`data-id`)
        };

        Global.post(`Project_DeleteMember`, vm)
            .done(function (data) {

                $(`m-card[data-id="${vm.token}"]`).remove();

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(1, `Error`, data.responseJSON.Message, `error`);
            });

    };

    const _getByPage = function (page) {

        let options = ``;
        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort
        }

        if (page == 1) _self.arr = [];

        //$(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        //$(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        _self.page = page;
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.arr.push(obj);
                
                $(`m-select[data-name="${_self.name}"]`).remove();

                if ($(`#txtSearch${_self.name}`).val() == ``)
                    return;

                for (let obj of _self.arr)
                    options += `<m-option data-name="${_self.name}">${obj.email}</m-option>`;

                $(`#txtSearch${_self.name}`).parent().append(`<m-select data-name="${_self.name}">${options}</m-select>`);

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };

    const _search = function (e, $this) {

        if (e.which == 13) {

            $(`m-select[data-name="${_self.name}"]`).remove();
            _add();

            return;

        }

        _getByPage(1);

    };
    const _select = function ($this) {

        $(`#txtSearch${_self.name}`).val($this.html()).focus();
        $(`m-select[data-name="${_self.name}"]`).remove();

    };
    //Public -------------------------------------------------

    const getHtmlBody = function () {

        let html = ``;

        for (let obj of Project.getSelf().vm.members)
            html += getHtmlCard(obj);

        return `

            <m-flex data-type="col" class="w" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">Members</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                    </m-flex>

                </m-flex>
            
                ${html}

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB" data-id="${obj.token}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.email}
                    </h2>
                    ${Global.jack.mIM ? `
                    <m-flex data-type="row" class="n c sm sQ primary btnDeleteMember" data-id="${obj.token}">
                        <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                    </m-flex>` : ``}
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `.btnDeleteMember`, function () { _delete($(this)); });
        $(document).on(`tap`, `m-option[data-name="${_self.name}"]`, function () { _select($(this)); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function (e) { _search(e, $(this)); });
    })();

    return {
        getHtmlBody: getHtmlBody,
        getHtmlCard: getHtmlCard
    };

})();