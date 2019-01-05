'use strict';

const Members = (function () {

    //Private ------------------------------------------------
    const _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        arr: [],
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        name: `Member`,
        constructor: function (memberId, name, title, companyName, phone1, phone2,
            skypeId, email, companyTemp, resume, portfolio, personalWebsite, skills,
            isEdcFamily, isPotentialStaffing, isDeleted) {
            this.memberId = memberId;
            this.name = name;
            this.title = title;
            this.companyName = companyName;
            this.phone1 = phone1;
            this.phone2 = phone2;
            this.skypeId = skypeId;
            this.email = email;
            this.companyTemp = companyTemp;
            this.resume = resume;
            this.portfolio = portfolio;
            this.personalWebsite = personalWebsite;
            this.skills = skills
            this.isEdcFamily = isEdcFamily;
            this.isPotentialStaffing = isPotentialStaffing;
            this.isDeleted = isDeleted;
        }
    }

    const _addEdit = function () {

        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.title = $(`#txtTitle${_self.name}`).val();
        _self.vm.companyName = $(`#txtCompanyName${_self.name}`).val();
        _self.vm.phone1 = $(`#txtPhone1${_self.name}`).val();
        _self.vm.phone2 = $(`#txtPhone2${_self.name}`).val();
        _self.vm.skypeId = $(`#txtSkypeId${_self.name}`).val();
        _self.vm.email = $(`#txtEmail${_self.name}`).val();
        _self.vm.companyTemp = $(`#txtCompanyTemp${_self.name}`).val();
        _self.vm.resume = $(`#txtResume${_self.name}`).val();
        _self.vm.portfolio = $(`#txtPortfolio${_self.name}`).val();
        _self.vm.personalWebsite = $(`#txtPersonalWebsite${_self.name}`).val();
        _self.vm.skills = $(`#txtSkills${_self.name}`).val();
        _self.vm.isEdcFamily = $(`#chkIsEDCFamily${_self.name}`).prop(`checked`);
        _self.vm.isPotentialStaffing = $(`#chkIsPotentialStaffing${_self.name}`).prop(`checked`);
        _self.vm.isDeleted = false;

        _addEditDelete();

    }
    const _addEditDelete = function () {

        try {

            Validation.getIsValidForm($('m-module'));

            Global.post(`Member_AddEditDelete`, _self.vm)
                .done(function (data) {
                    Validation.done();

                    _self.arr = [];

                    _getByPage(1);

                    Validation.notification(1);
                    Module.closeModuleAll();
                })
                .fail(function (data) {
                    Validation.fail(data);
                });

        } catch (ex) {
            Validation.fail(ex);
        }

    }

    const _delete = function () {

        _self.vm.isDeleted = true;

        _addEditDelete();

    }

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
        Global.post(`Member_GetByPage`, vm)
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

        Global.post(`Member_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, false, false, false);
    }

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    }
    const _sort = function ($this) {

        var dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    }

    //Public ------------------------------------------------
    const getHtmlModuleAdd = function () {
        _self.vm = _getEmptyVM();
        return `

            <m-header data-label="${_self.name} Add Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h active">
                        <span>Add</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body data-label="${_self.name} Add Body">

                <m-flex data-type="col" class="n pL pR">

                    <m-flex data-type="col" class="n">
                        <h1>${_self.name}</h1>
                        <label>Add</label>
                    </m-flex>
                
                </m-flex>

                ${getHtmlBodyForm()}

                <m-flex data-type="row" class="footer">
                    <m-button data-type="secondary" class="btnCloseModule">
                        Cancel
                    </m-button>

                    <m-button data-type="primary" id="btnAdd${_self.name}">
                        Add
                    </m-button>
                </m-flex>

            </m-body>
        
            `;
    }
    const getHtmlModuleDetail = function (id) {
        _getById(id);
        return `

            <m-header data-label="${_self.name} Detail Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body data-label="${_self.name} Detail Body">

                <m-flex data-type="col">

                    <h1 class="loading">Loading</h1>
                
                </m-flex>

            </m-body>
        
            `;
    }

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        <m-flex data-type="row" class="n c sm sQ primary btnOpenModule" data-function="Members.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>

                        <!--<m-flex data-type="row" class="n pL pR">

                            <m-button data-type="primary" class="" id="btnUpload${_self.name}">
                                Upload
                            </m-button>
                            <input type="file" class="none" id="upl${_self.name}" />

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
                    <h2 class="${Global.getSort(_self.sort, `firstName`)}" data-sort="firstName">
                        First Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `lastName`)}" data-sort="lastName">
                        Last Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `email`)}" data-sort="email">
                        Email
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
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="member,btnDelete${_self.name}">
                    <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                </m-flex>
            
            </m-flex>

            ${getHtmlBodyForm()}

            <m-flex data-type="row" class="footer">
                <m-button data-type="secondary" class="btnCloseModule">
                    Cancel
                </m-button>

                <m-button data-type="primary" id="btnEdit${_self.name}">
                    Save
                </m-button>
            </m-flex>

            `;
    }
    const getHtmlBodyForm = function () {
        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtFirstName${_self.name}">First Name</label>
                        <input type="text" id="txtFirstName${_self.name}" placeholder="First Name" value="${_self.vm.firstName}" required />
                    </m-input>

                    <m-input>
                        <label for="txtLastName${_self.name}">Last Name</label>
                        <input type="text" id="txtLastName${_self.name}" placeholder="Last Name" value="${_self.vm.lastName}" required />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtEmail${_self.name}">Email</label>
                        <input type="text" id="txtEmail${_self.name}" placeholder="Email" value="${_self.vm.email}" />
                    </m-input>

                    <m-input>
                        <label for="txtPhone${_self.name}">Phone</label>
                        <input type="text" id="txtPhone${_self.name}" placeholder="Phone" value="${_self.vm.phone}" />
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="Members.getHtmlModuleDetail" data-args="${obj.memberId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.firstName}
                    </h2>
                    <h2 class="tE">
                        ${obj.lastName}
                    </h2>
                    <h2 class="tE">
                        ${obj.email}
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
        $(document).on(`tap`, `#txtSearch${_self.name}`, function () { _search(); });
    })();

    return {
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    }

})();