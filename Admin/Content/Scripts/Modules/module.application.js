'use strict';

const Application = (function () {

    //Private ------------------------------------------------
    let _self = {
        name: `Application`
    };

    const _open = function () {
        $(`body`).prepend(Application.getHtml());
        Module.openBody(`Primary`, `Application.getHtmlBody`);
        _openSub($(`#initOpenSub`));
    }
    const _openSub = function ($this) {
        if ($(`m-sub[data-label="${$this.html()}"]`).is(`:visible`)) return;
        $(`m-sub[data-label="${$this.html()}"]`).velocity(`stop`).velocity('slideDown', { duration: Global.velocitySettings.durationShort });
        $this.addClass(`btnCloseSub`).removeClass(`btnOpenSub`);
    }
    const _closeSub = function ($this) {
        if (!$(`m-sub[data-label="${$this.html()}"]`).is(`:visible`)) return;
        $(`m-sub[data-label="${$this.html()}"]`).velocity(`stop`).velocity('slideUp', { duration: Global.velocitySettings.durationShort });
        $this.addClass(`btnOpenSub`).removeClass(`btnCloseSub`);
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
                <m-flex data-type="col" class="n">

                    <m-flex data-type="row" class="">
                        <!--<m-image class="icon sm contain tR" style="background-image: url('https://authentication.ciclops.software/Content/Images/logo_icon_white.png');"></m-image>-->
                        <m-flex data-type="col" class="w n tE">
                            <h1>Admin Site</h1>
                            <h2 class="tE">${Global.jack.mE}</h2>
                        </m-flex>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Log.getHtmlBody" tabindex="0" role="tab" data-label="Logs">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-notification"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-notification"></use></svg></i>
                        </m-flex>
                        <span>Activity<span>
                    </m-flex>

                    <label class="btnOpenSub" id="initOpenSub">Management</label>

                    <m-sub data-label="Management" style="display: none">
                        <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Members.getHtmlBody" tabindex="0" role="tab" data-label="Members">
                            <m-flex data-type="row" class="n c sm sQ">
                                <i class="icon-male-user"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-male-user"></use></svg></i>
                            </m-flex>
                            <span>Members<span>
                        </m-flex>
                    </m-sub>

                </m-flex>

                <m-flex data-type="col" class="n">

                    <m-flex data-type="row" class="n sC h" id="btnSignOut" tabindex="0" role="tab" data-label="Sign Out">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-padlock"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-padlock"></use></svg></i>
                        </m-flex>
                        <span>Sign Out<span>
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
        $(document).on(`tap`, `.btnOpenSub`, function () { _openSub($(this)); });
        $(document).on(`tap`, `.btnCloseSub`, function () { _closeSub($(this)); });

        _getJackSparrow();

    })();

    return {
        getHtml: getHtml,
        getHtmlBody: getHtmlBody
    }

})();

const Settings = (function () {

    //Private -------------------------------------------
    let _self = {
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