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

        //Load Lists
        MaterialTag.get();
        TimeTracker.getIsActive();

    }
    const _openSub = function ($this) {
        if ($(`m-sub[data-label="${$this.html()}"]`).is(`:visible`)) return;
        $(`m-sub[data-label="${$this.html()}"]`).velocity(`stop`).velocity('slideDown', { duration: Global.velocitySettings.durationShort });
        $this.addClass(`btnCloseSub`).removeClass(`btnOpenSub`);
    }
    const _openMenu = function ($this) {

        $(`m-body[data-label="Primary"]`).removeClass(`minimized`);
        $(`m-navigation`).removeClass(`minimized`);
        $(`.btnOpenMenu`).removeClass(`btnOpenMenu`).addClass(`btnCloseMenu`);
        Global.editIcon($this, `icon-back`);

    };
    const _closeSub = function ($this) {
        if (!$(`m-sub[data-label="${$this.html()}"]`).is(`:visible`)) return;
        $(`m-sub[data-label="${$this.html()}"]`).velocity(`stop`).velocity('slideUp', { duration: Global.velocitySettings.durationShort });
        $this.addClass(`btnOpenSub`).removeClass(`btnCloseSub`);
    };
    const _closeMenu = function ($this) {

        $(`m-body[data-label="Primary"]`).addClass(`minimized`);
        $(`m-navigation`).addClass(`minimized`);
        $(`.btnCloseMenu`).removeClass(`btnCloseMenu`).addClass(`btnOpenMenu`);
        Global.editIcon($this, `icon-forward`);

    };

    const _getJackSparrow = function () {

        $.ajax({
            type: "POST",
            url: '/Methods/GetJackSparrow',
            dataType: "json",
            success: function (data) {
                console.log(data);
                Global.jack = data;
                _open();
            }
        });

    };

    //Public -------------------------------------------------
    const getHtml = function () {
        return `

            <m-navigation class="minimized">
                <m-flex data-type="col" class="n">

                    <m-flex data-type="row" class="n sC">
                        <!--<m-image class="icon sm contain tR" style="background-image: url('https://authentication.ciclops.software/Content/Images/logo_icon_white.png');"></m-image>-->
                        <m-flex data-type="col" class="w n tE">
                            <h1>Welcome back!</h1>
                            <h2 class="tE">${Global.jack.mE}</h2>
                        </m-flex>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Application.getHtmlBody" tabindex="0" role="tab" data-label="Dashboard">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-home"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-home"></use></svg></i>
                        </m-flex>
                        <span class="tE">Dashboard<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Contacts.getHtmlBody" tabindex="0" role="tab" data-label="Contacts">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-male-user"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-male-user"></use></svg></i>
                        </m-flex>
                        <span class="tE">Contacts<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Materials.getHtmlBody" tabindex="0" role="tab" data-label="Materials">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-news"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-news"></use></svg></i>
                        </m-flex>
                        <span class="tE">Materials<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Settings.getHtmlBody" tabindex="0" role="tab" data-label="Settings">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-services"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-services"></use></svg></i>
                        </m-flex>
                        <span class="tE">Settings<span>
                    </m-flex>

                </m-flex>

                <m-flex data-type="col" class="n">

                    <m-flex data-type="row" class="n sC h" id="btnSignOut" tabindex="0" role="tab" data-label="Sign Out">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-padlock"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-padlock"></use></svg></i>
                        </m-flex>
                        <span class="tE">Sign Out<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenMenu">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-forward"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-forward"></use></svg></i>
                        </m-flex>
                        <span class="tE">Collapse<span>
                    </m-flex>

                </m-flex>

            </m-navigation>

            <m-body data-label="Primary" class="minimized">
            </m-body>

            `;
    };

    const getHtmlBody = function () {
        return `

            <m-flex data-type="col" class="container">

                <m-flex data-type="row" class="n">

                    ${Project.getHtmlBody()}

                    <m-flex data-type="col" class="w">
                        Info
                    </m-flex>

                </m-flex>

            </m-flex>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `.btnOpenSub`, function () { _openSub($(this)); });
        $(document).on(`tap`, `.btnCloseSub`, function () { _closeSub($(this)); });
        $(document).on(`tap`, `.btnOpenMenu`, function () { _openMenu($(this)); });
        $(document).on(`tap`, `.btnCloseMenu`, function () { _closeMenu($(this)); });

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

                        <m-card class="mB btnOpenBody" data-label="Settings Body" data-function="TimeTracker.getHtmlBodyMember">
                            <m-flex class="c" data-type="row">
                                <h2>Time Sheet</h2>
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

    const _init = (function () {

    })();

    return {
        getHtmlBody: getHtmlBody,
        getHtmlBodyAccount: getHtmlBodyAccount,
        getHtmlBodyPublishLog: getHtmlBodyPublishLog
    }

})();