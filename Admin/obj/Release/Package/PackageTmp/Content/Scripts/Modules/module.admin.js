'use strict';

const Application = (function () {

    //Private ------------------------------------------------
    const _self = {
        name: `Application`
    };

    const _open = function () {
        $(`body`).prepend(Application.getHtml());
        Module.openBody(`Primary`, `Application.getHtmlBody`);
    }

    const _getJackSparrow = function () {

        $.ajax({
            type: "POST",
            url: '/Methods/GetJackSparrow',
            dataType: "json",
            success: function (data) {
                //console.log(data);
                Global.jack = data;
                _open();
            }
        });

    };

    //Public -------------------------------------------------
    const getHtml = function () {
        return `

            <m-navigation>
                <m-flex data-type="row" class="sB w n container">
                    <m-flex data-type="row" class="n">
                        <img class="mR" src="https://authentication.bambino.software/Content/Images/logo_horizontal.png" />
                        <m-flex data-type="row" class="n sC h pL pR active btnOpenBody" data-label="Primary" data-function="Application.getHtmlBody" tabindex="0" role="tab" data-label="Dashboard">
                            <span>Dashboard<span>
                        </m-flex>
                        <m-flex data-type="row" class="n sC h pL pR btnOpenBody" data-label="Primary" data-function="Settings.getHtmlBody" tabindex="0" role="tab" data-label="Settings">
                            <span>Settings<span>
                        </m-flex>
                    </m-flex>
                    <m-flex data-type="row" class="n">
                        <h2 class="mR">${Global.jack.mE}</h2>
                        <m-flex data-type="row" class="n sC h pL pR" id="btnSignOut" tabindex="0" role="tab" data-label="Sign Out">
                            <span>Sign Out<span>
                        </m-flex>
                    </m-flex>
                </m-flex>
            </m-navigation>

            <m-body data-label="Primary">
            </m-body>

            `;
    };

    const getHtmlBody = function () {
        return `

            <m-flex data-type="col" class="container">

                Hello

            </m-flex>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, ``, function () { });

        _getJackSparrow();

    })();

    return {
        getHtml: getHtml,
        getHtmlBody: getHtmlBody
    }

})();

const Settings = (function () {

    //Private -------------------------------------------
    const _self = {
        name: `Settings`
    };

    //Public --------------------------------------------
    const getHtmlBody = function () {
        return `

            <m-flex data-type="col" class="container" id="">

                <m-flex data-type="col" class="n mB">

                    <h1 class="lg">${_self.name}</h1>

                </m-flex>

                <m-flex data-type="row" class="s n">
        
                    <m-flex data-type="col" class="n cards selectable" style="min-width: 200px;">

                        <m-card class="mB active btnOpenBody" data-label="Settings Body" data-function="Settings.getHtmlBodyAccount">
                            <m-flex class="c" data-type="row">
                                <h2>Account</h2>
                            </m-flex>
                        </m-card>

                        <m-card class="mB btnOpenBody" data-label="Settings Body" data-function="Settings.getHtmlBodyPublishLog">
                            <m-flex class="c" data-type="row">
                                <h2>Publish Log</h2>
                            </m-flex>
                        </m-card>

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
                        
                        <m-flex data-type="row" class="n">

                            <m-input class="mR">
                                <label for="txtFirstName">First Name</label>
                                <input type="text" id="txtFirstName" placeholder="First Name" value="${Global.jack.mFN}" />
                            </m-input>

                            <m-input>
                                <label for="txtLastName">Last Name</label>
                                <input type="text" id="txtLastName" placeholder="Last Name" value="${Global.jack.mLN}" />
                            </m-input>

                        </m-flex>

                        <m-flex data-type="row" class="n">

                            <m-input class="mR">
                                <label for="txtEmail">Email</label>
                                <input type="email" id="txtEmail" placeholder="Email" value="${Global.jack.mE}" required />
                            </m-input>

                            <m-input>
                                <label for="txtPhone">Phone</label>
                                <input type="text" id="txtPhone" placeholder="Phone" value="${Global.jack.mP}" />
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
                
                        <m-button data-type="primary" id="btnEditPassword">
                            Update Password
                        </m-button>

                    </m-flex>

                </m-flex>
                
                <m-flex data-type="col" class="n" id="flxEditMember">

                    <m-flex data-type="col">

                        <h1 class="w mB">API Integration</h1>
                        
                        <m-flex data-type="row" class="n">
                        
                            <m-flex data-type="row" class="sC w n" id="flxTokenApi">

                                <h2>
                                    **************
                                </h2>
                                
                            </m-flex>
                        
                            <m-button data-type="primary" class="mR" id="btnGetToken">
                                Show
                            </m-button>
                        
                            <m-button data-type="secondary" class="btnOpenModule" data-function="Application.getHtmlModuleResetTokenConfirmation" data-args="">
                                Reset
                            </m-button>

                        </m-flex>

                    </m-flex>
                    
                </m-flex>

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
                
                    <h1>1.0.0</h1>
                    <label class="mB">July 5th, 2018</label>

                    <h3>Bug Fixes</h3>
                    <ul class="mB">
                        <li>Maiden Name and Nickname added to final review</li>
                    </ul>

                    <h3>Updates</h3>
                    <ul class="mB">
                        <li>Nickname added to orders</li>
                    </ul>

                </m-flex>

            </m-flex>
        
            `;
    }

    const _init = (function () {

    })();

    return {
        getHtmlBody: getHtmlBody,
        getHtmlBodyAccount: getHtmlBodyAccount,
        getHtmlBodyPublishLog: getHtmlBodyPublishLog
    }

})();