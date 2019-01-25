'use strict';

const Members = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        arr: [],
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        name: `Member`,
        constructor: function (memberId, companyId, firstName, lastName, email, phone, isDeleted, roles) {
            this.memberId = memberId;
            this.companyId = companyId;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.phone = phone;
            this.isDeleted = isDeleted;
            this.roles = roles;
        }
    }

    const _edit = function () {

        _self.vm.companyId = $(`#dboCompanyId${_self.name}`).length > 0 ? $(`#dboCompanyId${_self.name}`).val() : Global.jack.mCI;
        _self.vm.firstName = $(`#txtFirstName${_self.name}`).val();
        _self.vm.lastName = $(`#txtLastName${_self.name}`).val();
        _self.vm.email = $(`#txtEmail${_self.name}`).val();
        _self.vm.phone = $(`#txtPhone${_self.name}`).val();
        _self.vm.isDeleted = false;

        _editDelete();

    }
    const _editPassword = function () {

        const vm = {
            passwordOld: $(`#txtCurrentPassword`).val(),
            passwordNew: $(`#txtNewPassword`).val()
        };

        try {

            Validation.getIsValidForm($('#flxEditPassword'));

            if ($(`#txtNewPassword`).val() !== $(`#txtNewConfirmPassword`).val())
                throw `Passwords do not match.`;

            Global.post(`${_self.name}_EditPassword`, vm)
                .done(function (data) {
                    Validation.done();
                    $(`#txtCurrentPassword`).val(``);
                    $(`#txtNewPassword`).val(``);
                    $(`#txtNewConfirmPassword`).val(``);
                    Validation.notification(1);
                })
                .fail(function (data) {
                    Validation.fail(data);
                });

        } catch (ex) {
            Validation.fail(ex);
        }

    };

    const _editDelete = function () {

        try {

            Validation.getIsValidForm($('m-module'));
            
            Global.post(`${_self.name}_EditDelete`, _self.vm)
                .done(function (data) {
                    Validation.done();

                    Global.jack.mFN = _self.vm.firstName;
                    Global.jack.mLN = _self.vm.lastName;
                    Global.jack.mE = _self.vm.email;
                    Global.jack.mP = _self.vm.phone;
                    Global.editJackSparrow();

                    _self.arr = [];

                    if ($(`#lstMembers`).length > 0)
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

        _editDelete();

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

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty, Global.guidEmpty, ``, ``, ``, ``, false, []);
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
    const _invite = function () {

        const vm = {
            email: $(`#txtEmail${_self.name}`).val()
        }

        try {

            Validation.getIsValidForm($('m-module'));

            Global.post(`${_self.name}_Invite`, vm)
                .done(function (data) {
                    Validation.done();
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

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };

    const getHtmlModuleAdd = function () {
        _self.vm = _getEmptyVM();
        return `

            <m-header data-label="${_self.name} Add Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h active">
                        <span>Invite</span>
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
                        <label>Invite</label>
                    </m-flex>
                
                </m-flex>

                <m-flex data-type="col" class="form">

                    <m-input class="mR">
                        <label for="txtEmail${_self.name}">Email</label>
                        <input type="text" id="txtEmail${_self.name}" placeholder="Email" value="${_self.vm.email}" />
                    </m-input>

                </m-flex>

                <m-flex data-type="row" class="footer">
                    <m-button data-type="secondary" class="btnCloseModule">
                        Cancel
                    </m-button>

                    <m-button data-type="primary" id="btnInvite${_self.name}">
                        Invite
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

        let html = ``;

        for (let role of _self.vm.roles)
            html += Role.getHtmlTag(role);

        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">

                    <m-input>
                        <label for="dboCompanyId${_self.name}">Company</label>
                        <select id="dboCompanyId${_self.name}">
                            ${Global.getHtmlOptions(Company.getSelf().arr, [_self.vm.companyId])}
                        </select>
                    </m-input>

                </m-flex>

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

                <m-flex data-type="row" class="n s">
                    <m-input class="mR">
                        <input type="text" id="txtSearchRole" placeholder="Role" value="" />
                    </m-input>

                    <m-flex data-type="row" class="n c sm sQ secondary btnAddRole">
                        <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                    </m-flex>
                </m-flex>

                <label for="txtSearchRole" class="mB">Roles</label>

                <m-flex data-type="row" class="n s" id="flxRoleTags">
                    ${html}
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
        $(document).on(`tap`, `#btnInvite${_self.name}`, function () { _invite(); });
        $(document).on(`tap`, `#btnEdit${_self.name}`, function () { _edit(); });
        $(document).on(`tap`, `#btnEditPassword${_self.name}`, function () { _editPassword(); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete(); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); });
        $(document).on(`tap`, `#txtSearch${_self.name}`, function () { _search(); });
    })();

    return {
        getSelf: getSelf,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    }

})();