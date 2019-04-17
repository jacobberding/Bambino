'use strict';

const Contact = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        arr: [],
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        name: `Contact`,
        constructor: function (contactKey, contactCompanyKey, name, title, phone1, phone2,
            skypeId, email, personalWebsite, skills,
            isEdcFamily, isPotentialStaffing, isDeleted, contactFiles) {
            this.contactKey = contactKey;
            this.contactCompanyKey = contactCompanyKey;
            this.name = name;
            this.title = title;
            this.phone1 = phone1;
            this.phone2 = phone2;
            this.skypeId = skypeId;
            this.email = email;
            this.personalWebsite = personalWebsite;
            this.skills = skills;
            this.isEdcFamily = isEdcFamily;
            this.isPotentialStaffing = isPotentialStaffing;
            this.isDeleted = isDeleted;
            this.contactFiles = contactFiles;
        }
    };

    const _addEdit = function () {

        _self.vm.contactCompanyKey = $(`#dboContactCompanyKey${_self.name}`).val();
        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.title = $(`#txtTitle${_self.name}`).val();
        _self.vm.phone1 = $(`#txtPhone1${_self.name}`).val();
        _self.vm.phone2 = $(`#txtPhone2${_self.name}`).val();
        _self.vm.skypeId = $(`#txtSkypeId${_self.name}`).val();
        _self.vm.email = $(`#txtEmail${_self.name}`).val();
        _self.vm.personalWebsite = $(`#txtPersonalWebsite${_self.name}`).val();
        _self.vm.skills = $(`#txtSkills${_self.name}`).val();
        _self.vm.isEdcFamily = $(`#chkIsEDCFamily${_self.name}`).prop(`checked`);
        _self.vm.isPotentialStaffing = $(`#chkIsPotentialStaffing${_self.name}`).prop(`checked`);
        _self.vm.isDeleted = false;

        _addEditDelete();

    };
    const _addEditDelete = function () {

        try {

            Validation.getIsValidForm($('m-module'));

            if (_self.vm.name == `` && _self.vm.companyName == ``)
                throw `Please fill out either the name or the company name.`;

            Global.post(`${_self.name}_AddEditDelete`, _self.vm)
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

    };

    const _delete = function () {

        _self.vm.isDeleted = true;

        _addEditDelete();

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

    };
    const _getByKey = function (key) {

        const vm = {
            key: key
        }

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;
                ContactCompany.getSelf().vm = data.contactCompany;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _getEmptyVM = function () {
        return new _self.constructor(0, 0, ``, ``, ``, ``, ``, ``, ``, ``, false, false, false, []);
    };
    const _getExport = function () {
        
        Global.post(`${_self.name}_GetReport`, {})
            .done(function (data) {
                _getCSV(data);
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getCSV = function (data) {

        //By Person

        let csvArray = [
            [`FirstName`,`LastName`, `PrimaryEmail`,`Skills`,`Company`]
        ];
        let lineArray = [];

        for (let obj of data)
            csvArray.push([obj.name.split(' ')[0], obj.name.split(' ')[1], obj.email, obj.skills, obj.company]);

        csvArray.forEach(function (csvRow, index) {

            for (var i = 0; i < csvRow.length; i++)
                csvRow[i] = csvRow[i].toString().replace(/,/g, "");

            lineArray.push(csvRow.join(","));

        });

        saveAs(new Blob([lineArray.join("\n")], { type: "text/plain;charset=utf-8" }), `Bambino Report_Contacts_${moment().format(`MMDDYYYY hhmmssa`)}.csv`);

    };

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
    const _uploadData = function (files) {

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
    const getSelf = function () {
        return _self;
    };

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
    };
    const getHtmlModuleDetail = function (key) {
        _getByKey(key);
        return `

            <m-header data-label="${_self.name} Detail Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyFiles">
                        <span>Files</span>
                    </m-flex>
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="ContactCompany.getHtmlBodyInformation">
                        <span>Company</span>
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
    };

    const getHtmlBody = function () {
        ContactCompany.get();
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">Contacts</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        <m-flex data-type="row" class="n c sm sQ mR primary btnOpenModule" data-function="${_self.name}.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>

                        <m-flex data-type="row" class="n c sm sQ secondary" id="btnExport${_self.name}">
                            <i class="icon-download"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-download"></use></svg></i>
                        </m-flex>

                        <!--<m-flex data-type="row" class="n pL pR">

                            <m-button data-type="primary" class="" id="btnUploadData${_self.name}">
                                Upload
                            </m-button>
                            <input type="file" class="none" id="uplData${_self.name}" />

                        </m-flex>-->

                    </m-flex>

                </m-flex>
            
                ${Global.getHtmlLoading()}

            </m-flex>

            `;
    };
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
                    <h2 class="${Global.getSort(_self.sort, `ContactCompany.name`)}" data-sort="ContactCompany.name">
                        Company
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `title`)}" data-sort="title">
                        Title
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
    };
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to DELETE this contact?,btnDelete${_self.name}">
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
    };
    const getHtmlBodyFiles = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Files</label>
                </m-flex>

            </m-flex>

            <m-flex data-type="col" class="">
                ${ContactFile.getHtmlBodyForm()}
            </m-flex>

            `;
    };
    const getHtmlBodyForm = function () {
        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>

                    <m-input class="">
                        <label for="dboContactCompanyKey${_self.name}">Company</label>
                        <select id="dboContactCompanyKey${_self.name}">
                            <option value="0">N/A</option>
                            ${Global.getHtmlOptions(ContactCompany.getSelf().arr, [_self.vm.contactCompanyKey])}
                        </select>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtTitle${_self.name}">Title</label>
                        <input type="text" id="txtTitle${_self.name}" placeholder="Title" value="${_self.vm.title}" />
                    </m-input>

                    <m-input class="">
                        <label for="txtEmail${_self.name}">Email</label>
                        <input type="text" id="txtEmail${_self.name}" placeholder="Email" value="${_self.vm.email}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtPhone1${_self.name}">Phone 1</label>
                        <input type="text" id="txtPhone1${_self.name}" placeholder="Phone 1" value="${_self.vm.phone1}" />
                    </m-input>

                    <m-input class="">
                        <label for="txtPhone2${_self.name}">Phone 2</label>
                        <input type="text" id="txtPhone2${_self.name}" placeholder="Phone 2" value="${_self.vm.phone2}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">

                        <label for="txtPersonalWebsite${_self.name}">Personal Website</label>

                        <m-flex data-type="row" class="n">
                            <input type="text" id="txtPersonalWebsite${_self.name}" placeholder="Personal Website" value="${_self.vm.personalWebsite}" />
                            <m-flex data-type="row" class="n c sQ h a" data-href="${_self.vm.personalWebsite}">
                                <i class="icon-advertisement-page"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-advertisement-page"></use></svg></i>
                            </m-flex>
                        </m-flex>

                    </m-input>

                    <m-input>
                        <label for="txtSkypeId${_self.name}">Skype Id</label>
                        <input type="text" id="txtSkypeId${_self.name}" placeholder="Skype Id" value="${_self.vm.skypeId}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input>
                        <label for="txtSkills${_self.name}">Skills</label>
                        <input type="text" id="txtSkills${_self.name}" placeholder="Skills" value="${_self.vm.skills}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <m-flex data-type="row" class="n">
                            <input type="checkbox" class="mR" id="chkIsEDCFamily${_self.name}" ${(_self.vm.isEdcFamily) ? `checked` : ``} />
                            <label for="chkIsEDCFamily${_self.name}">Is EDC Family</label>
                        </m-flex>
                    </m-input>

                    <m-input>
                        <m-flex data-type="row" class="n">
                            <input type="checkbox" class="mR" id="chkIsPotentialStaffing${_self.name}" ${(_self.vm.isPotentialStaffing) ? `checked` : ``} />
                            <label for="chkIsPotentialStaffing${_self.name}">Is Potential Staffing</label>
                        </m-flex>
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="${_self.name}.getHtmlModuleDetail" data-args="${obj.contactKey}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.email}
                    </h2>
                    <h2 class="tE">
                        ${obj.contactCompany.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.title}
                    </h2>
                    ${(Global.isMobile()) ? `` : `
                    <h2 class="tE">
                        ${obj.skills}
                    </h2>
                    <h2 class="tE">
                        <span class="a" data-href="${obj.personalWebsite}">${obj.personalWebsite}</span>
                    </h2>`}
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        //$(document).on(`tap`, `#btnUploadData${_self.name}`, function (e) { e.stopPropagation(); e.preventDefault(); $(`#uplData${_self.name}`).click(); });
        //$(document).on(`change`, `#uplData${_self.name}`, function () { _uploadData($(this).prop(`files`)); });
        $(document).on(`tap`, `#btnExport${_self.name}`, function () { _getExport($(this)); });
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`tap`, `#btnAdd${_self.name}, #btnEdit${_self.name}`, function () { _addEdit(); });
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
        getHtmlBodyFiles: getHtmlBodyFiles,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    };

})();
const ContactFile = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `name asc`,
        isShowMore: false,
        name: `ContactFile`,
        arr: [],
        vm: {},
        constructor: function (contactFileKey, contactKey, name, path, originalFileName, isDeleted) {
            this.contactFileKey = contactFileKey;
            this.contactKey = contactKey;
            this.name = name;
            this.path = path;
            this.originalFileName = originalFileName;
            this.isDeleted = isDeleted;
        }
    };

    const _addMany = function (arr) {

        for (let obj of arr)
            $(`#lst${_self.name}`).prepend(getHtmlCard(obj));

        const vm = {
            contactFiles: arr
        }

        Global.post(`${_self.name}_AddMany`, vm)
            .done(function (data) {
                Validation.notification(1);
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };

    const _delete = function (id) {

        const vm = {
            contactFileKey: id,
            isDeleted: true
        }

        Global.post(`${_self.name}_Delete`, vm)
            .done(function (data) {
                Validation.notification(1);

                $(`m-card[data-id="${id}"]`).remove();

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };

    const _upload = function (files) {

        if (files.length == 0 || $(`#btnUpload${_self.name}`).hasClass('disabled')) { _uploadReset(); return; }
        $(`#btnUpload${_self.name}`).addClass('disabled');

        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            console.log(Global.getIsValidFile(files[i])); //10MB
            if (files[i].size > 10000000) { Validation.notification(1, `File Upload Limit`, `File size is above 10MB in size, please reduce the file size before uploading.`, `error`); _uploadReset(); return; }
            if (Global.getIsValidFile(files[i]) == false) { Validation.notification(1, `File Upload Type`, `File extension is not allowed.`, `error`); _uploadReset(); return; }
            formData.append(files[i].name, files[i]);
        }

        console.log(`uploadSuccess`);
        Global.upload(formData, ContactFile.uploadSuccess, '/File/Upload');

    };
    const _uploadReset = function () {
        $(`#upl${_self.name}`).val(``);
        $(`#btnUpload${_self.name}`).removeClass(`disabled`);
    };
    const _download = function ($this) {

        console.log(`download`);
        window.open($this.attr(`data-path`));

    }

    //Public ------------------------------------------------
    const addMany = function (obj) {
        _addMany(obj);
    };

    const getSelf = function () {
        return _self;
    };

    const getHtmlBodyForm = function () {

        let html = ``;

        for (let obj of Contact.getSelf().vm.contactFiles)
            html += getHtmlCard(obj);

        return `

            <m-flex data-type="row" class="n">

                <m-button data-type="primary" class="" id="btnUpload${_self.name}">
                    Upload
                </m-button>
                <input type="file" class="none" id="upl${_self.name}" />

            </m-flex>

            <m-flex data-type="col" class="n pB w" id="lst${_self.name}">

                ${html}

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-flex data-type="row" class="n mT">

                <m-card class="mR h" data-id="${obj.contactFileKey}" id="btnDownload${_self.name}" data-path="${obj.path}">
                    <m-flex data-type="row" class="n pL">
                        <h6>
                            ${obj.originalFileName}
                        </h6>
                        <m-flex data-type="row" class="n">
                            <m-flex data-type="row" class="n c sm sQ tertiary">
                                <i class="icon-download"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-download"></use></svg></i>
                            </m-flex>
                        </m-flex>
                    </m-flex>
                </m-card>

                <m-flex data-type="row" class="n c sm sQ secondary" id="btnDelete${_self.name}" data-id="${obj.contactFileKey}">
                    <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                </m-flex>

            </m-flex>
            
            `;
    };

    const uploadSuccess = function (arr) {

        for (let obj of arr)
            obj["contactKey"] = Contact.getSelf().vm.contactKey;

        _addMany(arr);
        _uploadReset();

    };

    const _init = (function () {
        $(document).on(`tap`, `#btnUpload${_self.name}`, function (e) { e.stopPropagation(); e.preventDefault(); $(`#upl${_self.name}`).click(); });
        $(document).on(`change`, `#upl${_self.name}`, function () { _upload($(this).prop(`files`)); });
        $(document).on(`tap`, `#btnDownload${_self.name}`, function () { _download($(this)); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete($(this).attr(`data-id`)); });
    })();

    return {
        addMany: addMany,
        getSelf: getSelf,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard,
        uploadSuccess: uploadSuccess
    }

})();
const ContactCompany = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        pageArr: [],
        sort: `name asc`,
        isShowMore: false,
        name: `ContactCompany`,
        arr: [],
        vm: {},
        constructor: function (contactCompanyKey, name, email, phone, website, addressLine1, addressLine2,
            city, state, zip, isVendor, isClient, isDeleted) {
            this.contactCompanyKey = contactCompanyKey;
            this.name = name;
            this.email = email;
            this.phone = phone;
            this.website = website;
            this.addressLine1 = addressLine1;
            this.addressLine2 = addressLine2;
            this.city = city;
            this.state = state;
            this.zip = zip;
            this.isVendor = isVendor;
            this.isClient = isClient;
            this.isDeleted = isDeleted;
        }
    };

    const _addEdit = function () {

        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.email = $(`#txtEmail${_self.name}`).val();
        _self.vm.phone = $(`#txtPhone${_self.name}`).val();
        _self.vm.website = $(`#txtWebsite${_self.name}`).val();
        _self.vm.addressLine1 = $(`#txtAddressLine1${_self.name}`).val();
        _self.vm.addressLine2 = $(`#txtAddressLine2${_self.name}`).val();
        _self.vm.city = $(`#txtCity${_self.name}`).val();
        _self.vm.state = $(`#dboState${_self.name}`).val();
        _self.vm.zip = $(`#txtZip${_self.name}`).val();
        _self.vm.isVendor = $(`#chkIsVendor${_self.name}`).prop(`checked`);
        _self.vm.isClient = $(`#chkIsClient${_self.name}`).prop(`checked`);
        _self.vm.isDeleted = false;

        _addEditDelete();

    };
    const _addEditDelete = function () {

        try {

            Validation.getIsValidForm($('m-module'));

            Global.post(`${_self.name}_AddEditDelete`, _self.vm)
                .done(function (data) {
                    Validation.done();

                    _self.pageArr = [];

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

    };

    const _delete = function () {

        _self.vm.isDeleted = true;

        _addEditDelete();

    };

    const _get = function () {

        //if (_self.arr.length > 0)
        //    return;

        Global.post(`${_self.name}_Get`, {})
            .done(function (data) {
                _self.arr = data;
            }).fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort
        }

        if (page == 1) _self.pageArr = [];

        $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        _self.page = page;
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.pageArr.push(obj);

                $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
                $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(getHtmlBodyList(_self.pageArr));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getByKey = function (key) {

        const vm = {
            key: key
        };

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getEmptyVM = function () {
        return new _self.constructor(0, ``, ``, ``, ``, ``, ``, ``, ``, ``, false, false, false);
    };

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
    const get = function () {
        _get();
    };
    const getSelf = function () {
        return _self;
    };

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
                        <h1>Company</h1>
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
    };
    const getHtmlModuleDetail = function (id) {
        _getByKey(id);
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
    };

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">Companies</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        <m-flex data-type="row" class="n c sm sQ mR primary btnOpenModule" data-function="ContactCompany.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>

                    </m-flex>

                </m-flex>
            
                ${Global.getHtmlLoading()}

            </m-flex>

            `;
    };
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
                    <h2 class="${Global.getSort(_self.sort, `website`)}" data-sort="website">
                        Website
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `state`)}" data-sort="state">
                        State
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
    };
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>Company</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to DELETE this company?,btnDelete${_self.name}">
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
    };
    const getHtmlBodyInformation = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>Contact Company</h1>
                    <label>Information</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="${_self.name}.getHtmlModuleDetail" data-args="${_self.vm.contactCompanyKey}">
                    <i class="icon-edit"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-edit"></use></svg></i>
                </m-flex>
            
            </m-flex>

            <m-flex data-type="col" class="">

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label>Name</label>
                        <h2>${_self.vm.name}</h2>
                    </m-input>

                    <m-input class="">
                        <label>Email</label>
                        <h2>${_self.vm.email}</h2>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label>Phone</label>
                        <h2>${_self.vm.phone}</h2>
                    </m-input>

                    <m-input class="">
                        <label>Website</label>
                        <h2>${_self.vm.website}</h2>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label>Address Line 1</label>
                        <h2>${_self.vm.addressLine1}</h2>
                    </m-input>

                    <m-input class="">
                        <label>Address Line 2</label>
                        <h2>${_self.vm.addressLine2}</h2>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label>City</label>
                        <h2>${_self.vm.city}</h2>
                    </m-input>

                    <m-input class="mR">
                        <label>Zip</label>
                        <h2>${_self.vm.zip}</h2>
                    </m-input>

                    <m-input class="">
                        <label>State</label>
                        <h2>${_self.vm.state}</h2>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label>Vendor</label>
                        <h2>${_self.vm.isVendor ? `Yes` : `No`}</h2>
                    </m-input>

                    <m-input class="">
                        <label>Client</label>
                        <h2>${_self.vm.isClient ? `Yes` : `No`}</h2>
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    };
    const getHtmlBodyForm = function () {
        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>

                    <m-input class="">
                        <label for="txtEmail${_self.name}">Email</label>
                        <input type="text" id="txtEmail${_self.name}" placeholder="Email" value="${_self.vm.email}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtPhone${_self.name}">Phone</label>
                        <input type="text" id="txtPhone${_self.name}" placeholder="Phone" value="${_self.vm.phone}" />
                    </m-input>

                    <m-input class="">
                        <label for="txtWebsite${_self.name}">Website</label>
                        <input type="text" id="txtWebsite${_self.name}" placeholder="Website" value="${_self.vm.website}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtAddressLine1${_self.name}">Address Line 1</label>
                        <input type="text" id="txtAddressLine1${_self.name}" placeholder="Address Line 1" value="${_self.vm.addressLine1}" />
                    </m-input>

                    <m-input class="">
                        <label for="txtAddressLine2${_self.name}">Address Line 2</label>
                        <input type="text" id="txtAddressLine2${_self.name}" placeholder="Address Line 2" value="${_self.vm.addressLine2}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtCity${_self.name}">City</label>
                        <input type="text" id="txtCity${_self.name}" placeholder="City" value="${_self.vm.city}" />
                    </m-input>

                    <m-input class="mR">
                        <label for="txtZip${_self.name}">Zip</label>
                        <input type="text" id="txtZip${_self.name}" placeholder="Zip" value="${_self.vm.zip}" />
                    </m-input>

                    <m-input class="">
                        <label for="dboState${_self.name}">State</label>
                        <select id="dboState${_self.name}">
                            ${Global.getHtmlOptions(listStates, [_self.vm.state])}
                        </select>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <m-flex data-type="row" class="n">
                            <input type="checkbox" class="mR" id="chkIsVendor${_self.name}" ${(_self.vm.isVendor) ? `checked` : ``} />
                            <label for="chkIsVendor${_self.name}">Is Vendor</label>
                        </m-flex>
                    </m-input>

                    <m-input>
                        <m-flex data-type="row" class="n">
                            <input type="checkbox" class="mR" id="chkIsClient${_self.name}" ${(_self.vm.isClient) ? `checked` : ``} />
                            <label for="chkIsClient${_self.name}">Is Client</label>
                        </m-flex>
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="ContactCompany.getHtmlModuleDetail" data-args="${obj.contactCompanyKey}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.email}
                    </h2>
                    <h2 class="tE">
                        ${obj.website}
                    </h2>
                    <h2 class="tE">
                        ${obj.state}
                    </h2>
                </m-flex>
            </m-card>

            `;
    };

    const getHtmlTag = function (obj) {
        return `

            <m-card class="tag" data-id="${obj.contactCompanyKey}">
                <m-flex data-type="row" class="n">
                    <h1 class="tE">
                        ${obj.name}
                    </h1>
                    <m-flex data-type="row" class="n c xs sQ tertiary btnDelete${_self.name}" data-id="${obj.contactCompanyKey}">
                        <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                    </m-flex>
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `m-option[data-name="${_self.name}"]`, function () { _select($(this)); });
        $(document).on(`tap`, `.btnAdd${_self.name}`, function () { _add(); });
        $(document).on(`tap`, `.btnDelete${_self.name}`, function () { _delete($(this)); });
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`tap`, `#btnAdd${_self.name}, #btnEdit${_self.name}`, function () { _addEdit(); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete(); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); });
    })();

    return {
        get: get,
        getSelf: getSelf,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBodyInformation: getHtmlBodyInformation,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlCard: getHtmlCard,
        getHtmlTag: getHtmlTag
    };

})();