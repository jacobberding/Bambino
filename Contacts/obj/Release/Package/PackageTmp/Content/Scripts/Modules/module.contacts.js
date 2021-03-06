﻿'use strict';

const Contacts = (function () {

    //Private ------------------------------------------------
    const _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        arr: [],
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        name: `Contact`,
        constructor: function (contactId, name, title, companyName, phone1, phone2,
            skypeId, email, companyTemp, resume, portfolio, personalWebsite, skills,
            isEdcFamily, isPotentialStaffing, isDeleted) {
            this.contactId = contactId;
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

            Global.post(`Contact_AddEditDelete`, _self.vm)
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
        Global.post(`Contact_GetByPage`, vm)
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

        Global.post(`Contact_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty,``,``,``,``,``,``,``,``,``,``,``,``,false,false,false);
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
    const _upload = function (files) {
        console.log(`_upload`);

        const file = files[0];
        let reader = new FileReader();

        reader.readAsText(file);
        reader.onload = function (event) {

            const data = $.csv.toArrays(event.target.result);
            let vm = {
                contactViewModels: []
            }

            for (var row in data) {

                if (row == 0) continue;
                
                vm.contactViewModels.push({
                    name:                   data[row][0],
                    title:                  data[row][1],
                    companyName:            data[row][2],
                    phone1:                 data[row][3],
                    phone2:                 data[row][4],
                    skypeId:                data[row][5],
                    email:                  data[row][6],
                    companyTemp:            data[row][7],
                    resume:                 data[row][8],
                    portfolio:              data[row][9],
                    personalWebsite:        data[row][10],
                    skills:                 data[row][11],
                    isEdcFamily:            (data[row][12] == `TRUE`) ? true : false,
                    isPotentialStaffing:    (data[row][13] == `TRUE`) ? true : false,
                    //dateCreated:            moment(data[row][14]).format(`YYYY-MM-DD`)
                });

            }

            console.log(`vm`,vm);
            Global.post(`Contact_Upload`, vm)
                .done(function (data) {
                    Validation.notification(1);
                })
                .fail(function (data) {
                    Validation.notification(2);
                });

        };
        reader.onerror = function () { alert('Unable to read ' + file.fileName); };

    }

    //Public ------------------------------------------------
    const getHtmlModuleAdd = function () {
        _self.vm = _getEmptyVM();
        return `

            <m-header aria-label="${_self.name} Add Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h active">
                        <span>Add</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete-3"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete-3"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body aria-label="${_self.name} Add Body">

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

            <m-header aria-label="${_self.name} Detail Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete-3"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete-3"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body aria-label="${_self.name} Detail Body">

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

                        <m-flex data-type="row" class="n c sm sQ primary btnOpenModule" data-function="Contacts.getHtmlModuleAdd">
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
                    <h2 class="${Global.getSort(_self.sort, `name`)}" data-sort="name">
                        Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `email`)}" data-sort="email">
                        Email
                    </h2>
                    ${(Global.isMobile()) ? `` : `
                    <h2 class="${Global.getSort(_self.sort, `skills`)}" data-sort="skills">
                        Skills
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `personalWebsite`)}" data-sort="personalWebsite">
                        Personal Website
                    </h2>`}
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

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="contact,btnDelete${_self.name}">
                    <i class="icon-delete-2"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete-2"></use></svg></i>
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

                <m-input>
                    <label for="txtName${_self.name}">Name</label>
                    <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" required />
                </m-input>

                <m-input>
                    <label for="txtTitle${_self.name}">Title</label>
                    <input type="text" id="txtTitle${_self.name}" placeholder="Title" value="${_self.vm.title}" />
                </m-input>

                <m-input>
                    <label for="txtCompanyName${_self.name}">Company Name</label>
                    <input type="text" id="txtCompanyName${_self.name}" placeholder="Company Name" value="${_self.vm.companyName}" />
                </m-input>

                <m-input>
                    <label for="txtPhone1${_self.name}">Phone 1</label>
                    <input type="text" id="txtPhone1${_self.name}" placeholder="Phone 1" value="${_self.vm.phone1}" />
                </m-input>

                <m-input>
                    <label for="txtPhone2${_self.name}">Phone 2</label>
                    <input type="text" id="txtPhone2${_self.name}" placeholder="Phone 2" value="${_self.vm.phone2}" />
                </m-input>

                <m-input>
                    <label for="txtSkypeId${_self.name}">Skype Id</label>
                    <input type="text" id="txtSkypeId${_self.name}" placeholder="Skype Id" value="${_self.vm.skypeId}" />
                </m-input>

                <m-input>
                    <label for="txtEmail${_self.name}">Email</label>
                    <input type="text" id="txtEmail${_self.name}" placeholder="Email" value="${_self.vm.email}" />
                </m-input>

                <m-input>
                    <label for="txtCompanyTemp${_self.name}">Company (Temp)</label>
                    <input type="text" id="txtCompanyTemp${_self.name}" placeholder="Company (Temp)" value="${_self.vm.companyTemp}" />
                </m-input>

                <m-input>
                    <label for="txtResume${_self.name}">Resume</label>
                    <input type="text" id="txtResume${_self.name}" placeholder="Resume" value="${_self.vm.resume}" />
                </m-input>

                <m-input>
                    <label for="txtPortfolio${_self.name}">Portfolio</label>
                    <input type="text" id="txtPortfolio${_self.name}" placeholder="Portfolio" value="${_self.vm.portfolio}" />
                </m-input>

                <m-input>
                    <label for="txtPersonalWebsite${_self.name}">Personal Website</label>
                    <input type="text" id="txtPersonalWebsite${_self.name}" placeholder="Personal Website" value="${_self.vm.personalWebsite}" />
                </m-input>

                <m-input>
                    <label for="txtSkills${_self.name}">Skills</label>
                    <input type="text" id="txtSkills${_self.name}" placeholder="Skills" value="${_self.vm.skills}" />
                </m-input>

                <m-input>
                    <input type="checkbox" class="mR" id="chkIsEDCFamily${_self.name}" ${(_self.vm.isEdcFamily) ? `checked` : ``} />
                    <label for="chkIsEDCFamily${_self.name}">Is EDC Family</label>
                </m-input>

                <m-input>
                    <input type="checkbox" class="mR" id="chkIsPotentialStaffing${_self.name}" ${(_self.vm.isPotentialStaffing) ? `checked` : ``} />
                    <label for="chkIsPotentialStaffing${_self.name}">Is Potential Staffing</label>
                </m-input>

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="Contacts.getHtmlModuleDetail" data-args="${obj.contactId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.email}
                    </h2>
                    ${(Global.isMobile()) ? `` : `
                    <h2 class="tE">
                        ${obj.skills}
                    </h2>
                    <h2 class="tE">
                        <a href="${obj.personalWebsite}">${obj.personalWebsite}</a>
                    </h2>`}
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        //$(document).on(`tap`, `#btnUpload${_self.name}`, function (e) { e.stopPropagation(); e.preventDefault(); $(`#upl${_self.name}`).click(); });
        //$(document).on(`change`, `#upl${_self.name}`, function () { _upload($(this).prop(`files`)); });
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