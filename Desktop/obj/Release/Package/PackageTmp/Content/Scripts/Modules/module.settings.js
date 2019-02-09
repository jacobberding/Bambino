'use strict';

const Settings = (function () {

    //Private -------------------------------------------
    let _self = {
        name: `Settings`
    };

    const _editPath = function (obj) {

        const vm = {
            path: obj.path,
            originalFileName: obj.originalFileName
        }

        //Global.jack.mP = vm.path;
        //Global.jack.mO = vm.originalFileName;
        //Global.editJackSparrow();

        $(`m-icon[data-type="member"]`).css(`background-image`, `url('${vm.path}')`);

        Global.post(`Member_EditPath`, vm)
            .done(function (data) {
                Validation.notification(1);
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    }

    const _upload = function (files) {

        if (files.length == 0 || $(`#btnUpload${_self.name}`).hasClass('disabled')) { _uploadReset(); return; }
        $(`#btnUpload${_self.name}`).addClass('disabled');

        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            console.log(Global.getIsValidFile(files[i])); //5MB
            if (files[i].size > 5000000) { Validation.notification(1, `File Upload Limit`, `File size is above 5MB in size, please reduce the file size before uploading.`, `error`); _uploadReset(); return; }
            if (Global.getIsValidFile(files[i]) == false) { Validation.notification(1, `File Upload Type`, `File extension is not allowed.`, `error`); _uploadReset(); return; }
            formData.append(files[i].name, files[i]);
        }

        Global.upload(formData, Settings.uploadSuccess, '/File/Upload');

    }
    const _uploadReset = function () {
        $(`#upl${_self.name}`).val(``);
        $(`#btnUpload${_self.name}`).removeClass(`disabled`);
    }

    //Public --------------------------------------------
    const getHtmlBody = function () {
        return `

            <m-flex data-type="col" class="container" id="">

                <m-flex data-type="row" class="s n">
        
                    <m-flex data-type="col" class="n cards selectable" style="min-width: 200px;">

                        <m-link class="active btnOpenBody" data-label="Settings Body" data-function="Settings.getHtmlBodyAccount">
                            Account
                        </m-link>

                        <m-link class="btnOpenBody" data-label="Settings Body" data-function="TimeTracker.getHtmlBodyMember">
                            Time Sheet
                        </m-link>

                        <m-link class="mB btnOpenBody" data-label="Settings Body" data-function="Settings.getHtmlBodyPublishLog">
                            Publish Log
                        </m-link>

                    </m-flex>

                    <m-flex data-type="row" class="n s w">
                        <m-body data-label="Settings Body">
                            ${Settings.getHtmlBodyAccount()}
                        </m-body>
                    </m-flex>

                </m-flex>

            </m-flex>

            `;
    }
    const getHtmlBodyAccount = function () {
        return `

            <m-flex data-type="col" class="container-sm">

                <m-flex data-type="col" class="n" id="flxEditMember">

                    <m-flex data-type="col">

                        <h1 class="w mB">Account</h1>
                        
                        ${Settings.getHtmlBodyPhoto()}

                        <m-flex data-type="row" class="n">

                            <m-input class="mR">
                                <label for="txtFirstNameMember">First Name</label>
                                <input type="text" id="txtFirstNameMember" placeholder="First Name" value="${Global.jack.mFN}" />
                            </m-input>

                            <m-input>
                                <label for="txtLastNameMember">Last Name</label>
                                <input type="text" id="txtLastNameMember" placeholder="Last Name" value="${Global.jack.mLN}" />
                            </m-input>

                        </m-flex>

                        <m-flex data-type="row" class="n">

                            <m-input class="mR">
                                <label for="txtEmailMember">Email</label>
                                <input type="email" id="txtEmailMember" placeholder="Email" value="${Global.jack.mE}" required />
                            </m-input>

                            <m-input>
                                <label for="txtPhoneMember">Phone</label>
                                <input type="text" id="txtPhoneMember" placeholder="Phone" value="${Global.jack.mP}" />
                            </m-input>

                        </m-flex>

                    </m-flex>

                    <m-flex data-type="row" class="footer mB">
                
                        <m-button data-type="primary" id="btnEditMember">
                            Save
                        </m-button>

                    </m-flex>

                </m-flex>

                <m-flex data-type="col" class="n" id="flxEditPassword">

                    <m-flex data-type="col">

                        <h1 class="w mB">Update Password</h1>

                        <m-input class="mR">
                            <label for="txtCurrentPassword">Current Password</label>
                            <input type="password" id="txtCurrentPassword" placeholder="Current Password" value="" required />
                        </m-input>

                        <m-flex data-type="row" class="n">

                            <m-input class="mR">
                                <label for="txtNewPassword">New Password</label>
                                <input type="password" id="txtNewPassword" placeholder="New Password" value="" required />
                            </m-input>

                            <m-input>
                                <label for="txtNewConfirmPassword">Confirm New Password</label>
                                <input type="password" id="txtNewConfirmPassword" placeholder="Confirm New Password" value="" required />
                            </m-input>

                        </m-flex>

                    </m-flex>

                    <m-flex data-type="row" class="footer mB">
                
                        <m-button data-type="primary" id="btnEditPasswordMember">
                            Update Password
                        </m-button>

                    </m-flex>

                </m-flex>
                
            </m-flex>

            `;
    }
    const getHtmlBodyPhoto = function () {
        return `

            <m-flex data-type="col">

                ${Members.getHtmlIcon(Members.getSelf().vm)}

                <m-button data-type="secondary" id="btnUpload${_self.name}">
                    Upload
                </m-button>
                <input type="file" class="none" id="upl${_self.name}" />

                <m-bar class="progress" id="bar${_self.name}">
                    <span></span>
                </m-bar>

            </m-flex>

            `;
    }
    const getHtmlBodyPublishLog = function () {
        return `

            <m-flex data-type="col" class="container-sm">

                <m-flex data-type="col" class="n mB">

                    <h1>Publish Log</h1>

                </m-flex>

                <m-flex data-type="col" class="mB">
                
                    <h1>0.0.0</h1>
                    <label class="mB">January 1st, 2019</label>

                    <!--<h3>Bug Fixes</h3>
                    <ul class="mB">
                        <li>Published Authentication</li>
                    </ul>-->

                    <h3>Updates</h3>
                    <ul class="mB">
                        <li>Published Authentication</li>
                    </ul>

                </m-flex>

            </m-flex>
        
            `;
    }

    const uploadSuccess = function (arr) {
        _editPath(arr[0]);
        _uploadReset();
    }

    const _init = (function () {
        $(document).on(`tap`, `#btnUpload${_self.name}`, function (e) { e.stopPropagation(); e.preventDefault(); $(`#upl${_self.name}`).click(); });
        $(document).on(`change`, `#upl${_self.name}`, function () { _upload($(this).prop(`files`)); });
    })();

    return {
        getHtmlBody: getHtmlBody,
        getHtmlBodyAccount: getHtmlBodyAccount,
        getHtmlBodyPhoto: getHtmlBodyPhoto,
        getHtmlBodyPublishLog: getHtmlBodyPublishLog,
        uploadSuccess: uploadSuccess
    }

})();