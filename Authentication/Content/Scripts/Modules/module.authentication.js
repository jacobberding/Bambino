'use strict';

const Settings = (function () {
    
    const cookie = `_e`;
    const returnUrl = `https://desktop.bambino.software`;
    const name = `Bambino`;
    const logo = `/Content/Images/logo_icon.png`;

    return {
        cookie: cookie,
        returnUrl: returnUrl,
        name: name,
        logo: logo
    }

})();

const Application = (function () {

    //Private ----------------------------------------------------------
    
    const _editJackSparrow = function (value) {
        return $.ajax({
            type: "POST",
            url: '/Methods/SetJackSparrow',
            data: { value: JSON.stringify(value) },
            dataType: "json",
            success: function (data) {
                
                Application.deleteCookie(Settings.cookie);
                Application.addCookie(Settings.cookie, data, 365);
                        
                window.location.href = Application.returnUrl;
                
            }
        });
    }
    
    const _getHostName = function (url) {
        var match = url.match(/:\/\/(www[0-9]?\.)?(.[^/:]+)/i);
        if (match != null && match.length > 2 && typeof match[2] === 'string' && match[2].length > 0) {
        return match[2];
        }
        else {
            return null;
        }
    }
    const _getDomain = function (url) {
        var hostName = _getHostName(url);
        var domain = hostName;
    
        if (hostName != null) {
            var parts = hostName.split('.').reverse();
        
            if (parts != null && parts.length > 1) {
                domain = parts[1] + '.' + parts[0];
                
                if (hostName.toLowerCase().indexOf('.co.uk') != -1 && parts.length > 2) {
                  domain = parts[2] + '.' + domain;
                }
            }
        }
    
        return domain;
    }
    const _getJackSparrow = function () {
        return $.ajax({
            type: "POST",
            url: '/Methods/GetJackSparrow',
            dataType: "json"
        });
    }
    
    const _open = function () {
        
        if (window.location.href.includes(`ResetPassword`)) {
            $(`m-authentication`).append(ForgotPassword.getHtmlCardReset());
        } else {
            $(`m-authentication`).append(SignIn.getHtmlCard());
        }

        if (Application.getUrlParameter(`email`) != null) $(`#txtEmail`).val(Application.getUrlParameter(`email`));
        if (Application.getUrlParameter(`success`) != null) Validation.notification(0, `Success`, `${decodeURI(Application.getUrlParameter(`success`))}`, `success`);
        Application.returnUrl = (Application.getUrlParameter(`returnUrl`) != null) ? Application.getUrlParameter(`returnUrl`) : Settings.returnUrl;
        
    }

    const _keyUp = function (e) {
        
        if (e.which == 13)
            if (Application.type == 'SignIn.getHtmlCard')
                SignIn.signIn();
            else if (Application.type == 'SignUp.getHtmlCard')
                SignUp.signUp();
            else if (Application.type == 'ForgotPassword.getHtmlCard')
                ForgotPassword.forgotPassword();
            else if (Application.type == 'ForgotPassword.getHtmlCardReset')
                ForgotPassword.resetForgotPassword();

    }

    //Public ----------------------------------------------------------
    let returnUrl = Settings.returnUrl;
    let type = ``;
    const baseUrl = window.location.protocol + "//" + window.location.host;
    const init = function () {
        $(document).on('keyup', function (e) { _keyUp(e); });
        _open();
    };
    
    const addCookie = function (name, value, days) {
        var expires;

        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toGMTString();
        } else {
            expires = "";
        }
        document.cookie = name + "=" + value + expires + "; path=/;domain=." + _getDomain(window.location.href);
    }
    
    const editJackSparrow = function (value) {
        return _editJackSparrow(value);
    }

    const deleteCookie = function (name) {
        Application.addCookie(name, "", -1);
    }

    const getCookie = function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));
        }
        return null;
    }
    const getUrlParameter = function (sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;
    
        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');
    
            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    }
    const getJackSparrow = function () {
        return _getJackSparrow();
    }
    const getHtmlCardTerms = function () {
        return `

            <m-card class="d1" data-label="Terms" tabindex="0" role="region">

                <m-flex data-type="col" class="">

                    <m-h1>
                        Terms of Use
                    </m-h1>
                    <m-h2>
                        Uncaged Software Terms of Use
                    </m-h2>

                    <m-flex data-type="col" class="n">

                        <p>
                            Effective on January 1st, 2018
                            <br>
                            This Agreement applies to the order, purchase, and delivery of memorialization products
                            from Uncaged Software, LLC. (“Seller”) to you (“Buyer”), unless Buyer enters into
                            a separate written agreement with Seller.
                        </p>

                        <h3>Copyright Issues</h3>
                        
                        <p>
                            While we are not obligated to review User Submitted Materials for copyright infringement, 
                            we are committed to protecting copyrights and expect users of our Sites and Apps to do the same. 
                            The Digital Millennium Copyright Act of 1998 (the “DMCA”) provides recourse for copyright owners 
                            who believe that material appearing on the Internet infringes their rights under U.S. copyright law. 
                            If you believe in good faith that any material used or displayed on or through our Sites and Apps 
                            infringes your copyright, you (or your agent) may send us a notice requesting that the material be 
                            removed, or access to it blocked. The notice must include the following information: (a) a physical 
                            or electronic signature of a person authorized to act on behalf of the owner of an exclusive right 
                            that is allegedly infringed; (b) identification of the copyrighted work claimed to have been 
                            infringed (or if multiple copyrighted works are covered by a single notification, a representative 
                            list of such works); (c) identification of the material that is claimed to be infringing or the 
                            subject of infringing activity, and information reasonably sufficient to allow us to locate the 
                            material on our Sites and Apps; (d) the name, address, telephone number and email address (if available) 
                            of the complaining party; (e) a statement that the complaining party has a good faith belief that use of 
                            the material in the manner complained of is not authorized by the copyright owner, its agent or the 
                            law; and (f) a statement that the information in the notification is accurate and, under penalty of 
                            perjury, that the complaining party is authorized to act on behalf of the owner of an exclusive right 
                            that is allegedly infringed. If you believe in good faith that a notice of copyright infringement has 
                            been wrongly filed against you, the DMCA permits you to send us a counter-notice. Notices and 
                            counter-notices must meet the then-current statutory requirements imposed by the DMCA; see 
                            http://www.copyright.gov for details. DMCA notices and counter-notices regarding our Sites and 
                            Apps should be sent to:
                            <br><br>
                            Uncaged Software, LLC.
                            <br>
                            info@uncaged.software
                        </p>

                        <h3>Confidential Information</h3>

                        <p>
                            By accessing the Site (and in particular your Account), you may be permitted to access certain 
                            confidential and proprietary information, including but not limited to code, software, 
                            user interfaces, methods, pricing, and operations and any other information we 
                            designate as being confidential whether verbally or in writing (collectively, "Confidential 
                            Information"). You agree that you will only use Confidential Information as explicitly permitted 
                            in these Terms of Use and any use restrictions required as a condition to downloading any such 
                            Confidential Information, you will not disclose any of our Confidential Information to any third 
                            party. You agree that you will take reasonable efforts to protect our Confidential Information from 
                            disclosure to third parties. Upon our request, at any time, you agree that you will promptly return 
                            to us all of your copies of our Confidential Information. We shall be entitled to injunctive relief 
                            in the event of any unauthorized use or disclosure, whether or not intentional, by you or your 
                            affiliate of our Confidential Information.
                        </p>

                        <h3>Payment Terms</h3>

                        <p>
                            Payment is due at time of placement of an order made through the Uncaged Software
                            website for memorialization products.  Advertised prices are in U.S. dollars and do not
                            include shipping, handling and taxes unless otherwise noted.
                            Accepted forms of payment are limited to credit card or check, to be paid
                            to Uncaged Software, LLC.
                        </p>

                        <h3>Shipping and Title</h3>

                        <p>
                            Seller will arrange to ship product(s) to Buyer via United Postal Service, uninsured,
                            and only within the United States of America. Title and risk of loss to products pass
                            to Buyer when Seller's designated shipper delivers products to the address specified by
                            Buyer. Buyer must notify Seller of damaged or missing items from your order within 24
                            hours after you receive your product.
                        </p>

                        <h3>Return Policy</h3>

                        <p>
                            Buyer may return products 3 days after Buyer receives the ordered product in the event
                            of the Seller incorrectly fulfilling order. To return products Buyer can contact Uncaged 
                            Software via email at info@uncaged.software to receive a Return Authorization Number. Seller will
                            correct any errors and re-manufacture product and ship to Buyer within 72 hours of
                            notification of there being an error.  Seller will absorb any shipping, handling and
                            delivery fees to get Buyer corrected order.
                        </p>

                        <h3>Disclaimer of Warranties; Limitation of Liability</h3>

                        <p>
                            SELLER'S AND YOUR MAXIMUM LIABILITY TO THE OTHER IS LIMITED TO THE PURCHASE PRICE YOU
                            PAID FOR PRODUCTS OR SERVICES PLUS INTEREST AS ALLOWED BY LAW.  NEITHER YOU NOR SELLER
                            IS LIABLE TO THE OTHER IF WE ARE UNABLE TO PERFORM DUE TO EVENTS WE ARE NOT ABLE TO
                            CONTROL, SUCH AS ACTS OF GOD, OR FOR PROPERTY DAMAGE, PERSONAL INJURY, LOSS OF USE,
                            INTERRUPTION OF BUSINESS, LOST PROFITS, LOST DATA OR OTHER CONSEQUENTIAL, PUNITIVE OR
                            SPECIAL DAMAGES, HOWEVER CAUSED, WHETHER FOR BREACH OF WARRANTY, CONTRACT, TORT
                            (INCLUDING NEGLIGENCE), STRICT LIABILITY OR OTHERWISE, OTHER THAN THOSE DAMAGES THAT
                            ARE INCAPABLE OF LIMITATION, EXCLUSION OR RESTRICTION UNDER APPLICABLE LAW.  THIS
                            AGREEMENT GIVES YOU SPECIFIC LEGAL RIGHTS, AND YOU MAY ALSO HAVE OTHER RIGHTS THAT VARY
                            FROM JURISDICTION TO JURISDICTION.  SOME JURISDICTIONS DO NOT ALLOW LIMITATIONS ON HOW
                            LONG AN IMPLIED WARRANTY LASTS OR THE EXCLUSION OR LIMITATION OF INCIDENTAL OR
                            CONSEQUENTIAL DAMAGES, SO THE ABOVE LIMITATIONS OR EXCLUSIONS MAY NOT APPLY TO YOU.
                        </p>

                        <h3>Choice of Law</h3>

                        <p>
                            Buyer and Seller agree that all disputes arising out of this contract shall be
                            interpreted under the laws of the State of Ohio.
                        </p>

                        <h3>Acceptance</h3>

                        <p>
                            The Buyer and Seller agree that this contract will be considered signed when the
                            Buyer agrees to these terms while signing up to use Bambino Software. Buyer and Seller agree that this
                            contract, although executed electronically, has the same legal effect as a document
                            executed on paper.
                        </p>

                    </m-flex>

                    <m-h1>
                        Privacy Policy
                    </m-h1>
                    <m-h2>
                        Uncaged Software Privacy Policy
                    </m-h2>

                    <m-flex data-type="col" class="n">

                        <p>
                            Effective on January 1st, 2018
                            <br>
                            This privacy statement describes how Uncaged Software, LLC. collects and uses the
                            personal information you provide on our Web site: bambino.software. It also describes the
                            choices available to you regarding our use of your personal information and how you can
                            access and update this information.
                        </p>

                        <h2>Collection and Use of Personal Information</h2>

                        <p>
                            We collect the following personal information from you
                        </p>
                        <ul>
                            <li>
                                Contact Information including name, email address, mailing address, phone number
                            </li>
                            <li>
                                Billing Information including credit card number, and billing address
                            </li>
                            <li>
                                Financial Information such as Credit Card numbers
                            </li>
                        </ul>
                        <p>
                            We automatically gather information about your computer such as your IP address,
                            browser type, referring/exit pages, and operating system.
                            <br>
                            This information is used to:
                        </p>
                        <ul>
                            <li>
                                Fulfill orders
                            </li>
                            <li>
                                Send order confirmations
                            </li>
                        </ul>

                        <h2>Information Sharing</h2>

                        <p>
                            We will share your personal information with third parties only in the ways that are
                            described in this privacy statement. We do not sell your personal information to third
                            parties. We may provide your personal information to companies that provide services to
                            help us with our business activities such as shipping your order or offering customer
                            service. These companies are authorized to use your personal information only as necessary
                            to provide these services to us.
                            <br>
                            We may also disclose your personal information
                        </p>
                        <ul>
                            <li>
                                As required by law such as to comply with a subpoena, or similar legal process
                            </li>
                            <li>
                                When we believe in good faith that disclosure is necessary to protect our
                                rights, protect your safety or the safety of others, investigate fraud, or
                                respond to a government request,
                            </li>
                            <li>
                                When we believe in good faith that disclosure is necessary to protect our
                                rights, protect your safety or the safety of others, investigate fraud, or
                                respond to a government request,
                            </li>
                            <li>
                                If Uncaged Software, LLC. is involved in a merger, acquisition, or sale of
                                all or a portion of its assets, you will be notified via email and/or a
                                prominent notice on our Web site of any change in ownership or uses of your
                                personal information, as well as any choices you may have regarding your
                                personal information,
                            </li>
                            <li>
                                To any other third party with your prior consent to do so.
                            </li>
                        </ul>

                        <h2>Cookies and Other Tracking Technologies</h2>

                        <p>
                            We place cookies on your computer to track your cart and save order information to
                            process your order of custom memorialization products.
                            <br>
                            We use a third party to gather information about how you and others use our Web site.
                            For example, we will know how many users access a specific page and which links they
                            clicked on. We use this aggregated information to understand and optimize how our site
                            is used.
                        </p>
                        <p>
                            <b>Links to Other Web Sites</b>
                            <br>
                            Our Site may include links to other Web sites whose privacy practices may differ from
                            those of Uncaged Software, LLC. If you submit personal information to any of
                            those sites, your information is governed by their privacy statements. We encourage
                            you to carefully read the privacy statement of any Web site you visit.

                        </p>

                        <h2>Security</h2>

                        <p>
                            The security of your personal information is important to us. We follow generally
                            accepted industry standards to protect the personal information submitted to us,
                            both during transmission and once we receive it. No method of transmission over the
                            Internet, or method of electronic storage, is 100% secure, however. Therefore, we
                            cannot guarantee its absolute security. If you have any questions about security on
                            our Web site, you can contact us at info@uncaged.software.  If you use our shopping
                            cart, the transmission of sensitive information collected on our order form is encrypted
                            using secure socket layer technology (SSL). We utilize the Intuit Quickbooks payment 
                            system to process all transactions involving credit cards.
                        </p>

                        <h2>Additional Policy Information</h2>

                        <p>
                            Our Web site offers publicly accessible blogs or community forums. You should be aware
                            that any information you provide in these areas may be read, collected, and used by
                            others who access them.
                            <br><br>
                            <b>Correcting and Updating Your Personal Information</b>
                            <br>
                            To review and update your personal information to ensure it is accurate, contact us at
                            info@uncaged.software.
                            <br><br>
                            <b>Notification of Privacy Statement Changes</b>
                            <br>
                            We may update this privacy statement to reflect changes to our information practices.
                            If we make any material changes we will notify you by email (sent to the e-mail address
                            specified in your account) or by means of a notice on this Site prior to the change
                            becoming effective. We encourage you to periodically review this page for the latest
                            information on our privacy practices.
                        </p>

                        <h2>Contact Information</h2>

                        <p>
                            You can contact us about this privacy statement by phone or email:
                            <br>
                            513.661.6656
                            <br>
                            info@uncaged.software
                        </p>

                    </m-flex>

                </m-flex>

                <m-flex data-type="row" class="footer">

                    <m-button data-type="primary" class="btnReplaceCard" data-label="Terms" data-function="SignUp.getHtmlCard" data-args="">
                        Back
                    </m-button>

                </m-flex>

            </m-card>

            `;
    }

    return {
        returnUrl: returnUrl,
        type: type,
        baseUrl: baseUrl,
        init: init,
        addCookie: addCookie,
        editJackSparrow: editJackSparrow,
        deleteCookie: deleteCookie,
        getCookie: getCookie,
        getUrlParameter: getUrlParameter,
        getJackSparrow: getJackSparrow,
        getHtmlCardTerms: getHtmlCardTerms
    }

})();

const SignIn = (function () {
    
    //Private ----------------------------------------------------------
    let _isGoogle = false;
    
    const _signIn = function () {

        Application.getJackSparrow()
            .done(function (data) {
                Global.jack = data; 
                
                const vm = {
                    email: $(`#txtEmail`).val(),
                    password: $(`#txtPassword`).val(),
                    cI: Global.jack.cI,
                    v: Global.jack.v
                }
        
                try {
        
                    Validation.getIsValidForm($(`m-card`)); //pass in parent element
                    console.log(vm);
            
                    Global.post('Member_SignIn', vm)
                        .done(function (data) {
                            Validation.done(data);
                            if (data.iV) {
                                Application.editJackSparrow(data);
                            } else {
                                SignUp.token = data.mT;
                                Module.replaceCard(`Sign In`, `SignUp.getHtmlCardSuccess`, ``);
                            }
                        })
                        .fail(function (data) {
                            Validation.fail(data);
                        });
        
                } catch (ex) {
                    Validation.fail(ex);
                }
        
            });
        
    }
    const _signInGoogle = function (googleUser) {

        if (!_isGoogle)
            return;

        // Useful data for your client-side scripts:
        var profile = googleUser.getBasicProfile();
        console.log("ID: " + profile.getId()); // Don't send this directly to your server!
        console.log('Full Name: ' + profile.getName());
        console.log('Given Name: ' + profile.getGivenName());
        console.log('Family Name: ' + profile.getFamilyName());
        console.log("Image URL: " + profile.getImageUrl());
        console.log("Email: " + profile.getEmail());

        // The ID token you need to pass to your backend:
        var id_token = googleUser.getAuthResponse().id_token;
        console.log("ID Token: " + id_token);

        Application.getJackSparrow()
            .done(function (data) {
                Global.jack = data; 

                const vm = {
                    email: profile.getEmail(),
                    path: profile.getImageUrl(),
                    firstName: profile.getName().split(" ")[0],
                    lastName: profile.getName().split(" ")[1],
                    token: id_token,
                    v: Global.jack.v
                }

                try {

                    Validation.getIsValidForm($(`m-card`)); //pass in parent element

                    console.log(vm);
                    Global.post('Member_SignInGoogle', vm)
                        .done(function (data) {
                            Validation.done(data);
                            if (data.iV) {
                                Application.editJackSparrow(data);
                            } else {
                                SignUp.token = data.mT;
                                Module.replaceCard(`Sign In`, `SignUp.getHtmlCardSuccess`, ``);
                            }
                        })
                        .fail(function (data) {
                            Validation.fail(data);
                        });

                } catch (ex) {
                    Validation.fail(ex);
                }

            });

    }

    //Public ----------------------------------------------------------
    let email = ``;

    const isGoogle = function () {
        _isGoogle = true;
    }

    const signIn = function () {
        _signIn();
    }
    const signInGoogle = function (googleUser) {
        _signInGoogle(googleUser);
    }

    const getHtmlCard = function () {
        Application.type = `SignIn.getHtmlCard`;
        return `
            
            <m-card class="d1" data-label="Sign In" tabindex="0" role="region">
                <m-flex data-type="row" class="s">
                
                    <m-image class="icon mR tR" style="background-image: url('${Settings.logo}');">
                    </m-image>

                    <m-flex data-type="col" class="n">
                        <h1>${Settings.name}</h1>
                        <h2>Sign In</h2>
                    </m-flex>

                </m-flex>
                <m-flex data-type="col" class="">
                
                    <div class="g-signin2 mB w" onclick="SignIn.isGoogle()" data-onsuccess="onSignIn" data-theme="light"></div>
                    <!--<a href="#" onclick="signOut();">Sign out</a>-->

                    <label class="mB c">or</label>

                    <m-input>
                        <label for="txtEmail">Email</label>
                        <input type="text" id="txtEmail" placeholder="Email" required value="${SignIn.email}" />
                    </m-input>

                    <m-input>
                        <label for="txtPassword">Password</label>
                        <input type="password" id="txtPassword" placeholder="Password" required />
                    </m-input>

                    <m-a class="btnReplaceCard" data-label="Sign In" data-function="ForgotPassword.getHtmlCard" tabindex="0" role="tab" data-label="Forgot Password">Forgot your password? Click here to reset your password</m-a>

                </m-flex>
                <m-flex data-type="row" class="footer">

                    <m-button data-type="secondary" class="btnReplaceCard" data-label="Sign In" data-function="SignUp.getHtmlCard" data-args="">
                        Sign Up
                    </m-button>
                    <m-button data-type="primary" id="btnSignIn">
                        Sign In
                    </m-button>

                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        $(document).on(`tap`, `#btnSignIn`, function () { _signIn(); });
        $(document).on(`keyup`, `#txtEmail`, function () { SignIn.email = $(this).val(); });
    })();

    return {
        isGoogle: isGoogle,
        email: email,
        signIn: signIn,
        signInGoogle: signInGoogle,
        getHtmlCard: getHtmlCard
    }

})();
const SignUp = (function () {
    
    //Private ----------------------------------------------------------
    
    const _getReCaptcha = function (token) {
        $.ajax({
            type: "POST",
            url: '/Methods/GetCaptcha',
            data: {
                token: token
            },
            dataType: "json",
            success: function (data) {
                _signUp(data);
            }
        });
    }
    
    const _executeReCaptcha = function () {
        grecaptcha.execute();
    }
    const _signUp = function (data) {

        const vm = {
            email: $(`#txtEmail`).val(),
            password: $(`#txtPassword`).val(),
            companyId: Company.vm.companyId,
            pin: $(`#txtPin`).nval()
        }
        
        try {
        
            Validation.getIsValidForm($('m-card'));
        
            if (!data.success)
                throw `Not Valid.`;

            if (Company.vm.name == ``)
                throw `Please select a company.`;
            
            if (!$(`#chkIsAccepted`).prop(`checked`))
                throw `Please Accept the Terms & Conditions, and Privacy Policy.`;
            
            Global.post('Member_SignUp', vm)
                .done(function (data) {
                    Validation.done(data);
                    SignUp.token = data.token;
                    Validation.notification(0, `Success`, `Please check your email.`, `success`);
                    Module.replaceCard(`Sign Up`, `SignUp.getHtmlCardSuccess`, ``);
                })
                .fail(function (data) {
                    Validation.fail(data);
                });
        
        } catch (ex) {
            Validation.fail(ex);
            grecaptcha.reset();
        }
           
    }
    const _sendCode = function () {
        
        const vm = {
            token: SignUp.token
        };

        try {
            
            Validation.getIsValidForm($('m-card'));

            Global.post('Member_GetKeyCode', vm)
                .done(function (data) {
                    Validation.done(data);
                    Validation.notification(0, `Success`, `Code sent successfully.  Please check your email.`, `success`);
                    $('m-flex.footer').remove();
                })
                .fail(function (data) {
                    Validation.fail(data);
                });
        
        } catch (ex) {
            Validation.fail(ex);
        }

    }
    const _validate = function () {
        
        const vm = {
            token: SignUp.token,
            keyCode: $("#txtCodeOne").val() + $("#txtCodeTwo").val() + $("#txtCodeThree").val() + $("#txtCodeFour").val() + $("#txtCodeFive").val()
        };

        try {
        
            Validation.getIsValidForm($('m-card'));
            
            Global.post('Member_EditValidated', vm)
                .done(function (data) {
                    Validation.done(data);
                    Application.editJackSparrow(data);
                })
                .fail(function (data) {
                    Validation.fail(data);

                    $("#txtCodeOne, #txtCodeTwo, #txtCodeThree, #txtCodeFour, #txtCodeFive").val('');
                    $("#txtCodeOne").focus();
                        
                    if (data.responseJSON.Message.toLowerCase() == `token expired. please request another key code.`)
                        $('m-card').append(`
                            <m-flex data-type="row" class="footer">

                                <m-button data-type="primary" id="btnSendNewCode" tabindex="0" role="button" data-label="Send New Code">
                                    Send New Code
                                </m-button>

                            </m-flex>`);

                });
        
        } catch (ex) {
            Validation.fail(ex);
        }
    
    }
    
    const _keyUp = function (e) {
        
        const target = e.srcElement || e.target;
        const maxLength = parseInt(target.attributes["maxlength"].value, 10);
        const myLength = target.value.length;
        
        $('m-error').remove();

        if (myLength >= maxLength) {
            
            if (target.nextElementSibling == null)
                _validate();
            else 
                target.nextElementSibling.focus();
            
        }
        else if (myLength === 0) { // Move to previous field if empty (user pressed backspace)
            
            if (target.previousElementSibling != null)
                target.previousElementSibling.focus();

        }

    }
    
    //Public ----------------------------------------------------------
    let token = ``;
    let phone = ``;
    
    const getReCaptcha = function (token) {
        _getReCaptcha(token);
    }

    const getHtmlCard = function () {
        Application.type = `SignUp.getHtmlCard`;
        return `

            <m-card class="d1" style="overflow: initial;" data-label="Sign Up" tabindex="0" role="region">
                <m-flex data-type="row" class="s">
                
                    <m-image class="icon mR tR" style="background-image: url('${Settings.logo}');">
                    </m-image>

                    <m-flex data-type="col" class="n">
                        <h1>${Settings.name}</h1>
                        <h2>Sign Up</h2>
                    </m-flex>

                </m-flex>
                <m-flex data-type="col" class="">
                
                    <m-input class="mR">
                        <label for="txtEmail">Email</label>
                        <input type="text" id="txtEmail" placeholder="Email" required value="${SignIn.email}" />
                    </m-input>

                    <m-input class="mR">
                        <label for="txtPassword">Password</label>
                        <input type="password" id="txtPassword" placeholder="Password" required />
                    </m-input>

                    <m-input>
                        <label for="txtConfirmPassword">Confirm Password</label>
                        <input type="password" id="txtConfirmPassword" placeholder="Confirm Password" required />
                    </m-input>

                    <m-input>
                        <label for="txtCompany">Company</label>
                        <m-flex data-type="row" class="n">
                            <input type="text" id="txtCompany" placeholder="Company" />
                            <m-flex data-type="row" class="n c sQ h sm secondary">
                                <i class="icon-search"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-search"></use></svg></i>
                            </m-flex>
                        </m-flex>
                    </m-input>

                    <m-flex data-type="col" class="n mB cards selectable lstCompanies">
                        <h2>Selected Company</h2>
                        ${(Company.vm.name != ``) ? Company.getHtmlCard(Company.vm) : ``}
                    </m-flex>

                    <m-input class="mR">
                        <label for="txtPin">Pin</label>
                        <input type="number" id="txtPin" placeholder="Pin" required value="${SignIn.pin}" />
                    </m-input>

                    <m-input>
                        <m-flex data-type="row" class="n">
                            <label for="chkIsAccepted">I Accept the Terms & Conditions, and Privacy Policy</label>
                            <input type="checkbox" id="chkIsAccepted" />
                        </m-flex>
                    </m-input>

                    <m-a class="btnReplaceCard" data-label="Sign Up" data-function="Application.getHtmlCardTerms" tabindex="0" role="tab" data-label="Terms">By signing up, you agree to the Terms of Use & Privacy Policy</m-a>

                </m-flex>
                <m-flex data-type="row" class="footer">

                    <m-button data-type="secondary" class="btnReplaceCard" data-label="Sign Up" data-function="SignIn.getHtmlCard" data-args="">
                        Cancel
                    </m-button>
                    <m-button data-type="primary" id="btnSignUp">
                        Sign Up
                    </m-button>

                </m-flex>
            </m-card>

            `;
    }
    const getHtmlCardSuccess = function () {
        Application.type = `SignUp.getHtmlCardSuccess`;
        return `

            <m-card class="d1" tabindex="0" role="region" data-label="Sign Up Validation">
                <m-flex data-type="row" class="sC">

                    <m-image class="icon mR tR" style="background-image: url('${Settings.logo}');">
                    </m-image>

                    <m-flex data-type="col" class="n">
                        <h1>${Settings.name}</h1>
                        <h2>Account Validation</h2>
                    </m-flex>

                </m-flex>
                <m-flex data-type="col">

                    <m-inputs>
                        <input tabindex="0" role="textbox" onfocus="this.placeholder=''" onblur="this.placeholder='0'" data-label="First Number in Validation Code" type="number" placeholder="0" id="txtCodeOne" maxlength="1" />
                        <input tabindex="0" role="textbox" onfocus="this.placeholder=''" onblur="this.placeholder='0'" data-label="Second Number in Validation Code" type="number" placeholder="0" id="txtCodeTwo" maxlength="1" />
                        <input tabindex="0" role="textbox" onfocus="this.placeholder=''" onblur="this.placeholder='0'" data-label="Third Number in Validation Code" type="number" placeholder="0" id="txtCodeThree" maxlength="1" />
                        <input tabindex="0" role="textbox" onfocus="this.placeholder=''" onblur="this.placeholder='0'" data-label="Fourth Number in Validation Code" type="number" placeholder="0" id="txtCodeFour" maxlength="1" />
                        <input tabindex="0" role="textbox" onfocus="this.placeholder=''" onblur="this.placeholder='0'" data-label="Fifth Number in Validation Code" type="number" placeholder="0" id="txtCodeFive" maxlength="1" />
                    </m-inputs>

                    <h2 style="margin-bottom: .5em;text-align: center;">
                        Before you can enter Bambino we have to <span style="font-weight: 800;">validate your information</span>, we will be reaching out to you shortly.
                    </h2>

                    <h2 style="margin-bottom: .5em;text-align: center;">
                        Once we validate your information we ask that you <span style="font-weight: 800;">please verify your email</span> 
                        by entering the <span style="font-weight: 800;">code</span> we sent to your inbox in the spaces above.
                    </h2>

                    <h2 style="text-align: center;">
                        If you have any questions feel free to reach out to us directly at <span style="font-weight: 800;">513.661.6656</span>.
                    </h2>

                </m-flex>
            </m-card>

            `;
    }
    
    const signUp = function () {
        _signUp();
    }

    const _init = (function () {
        $(document).on(`tap`, `#btnSignUp`, function () { _signUp({ success: true }); });
        $(document).on(`tap`, '#btnSendNewCode', function () { _sendCode(); });
        $(document).on(`keyup`, 'm-card[data-label="Sign Up Validation"]', function (e) { _keyUp(e); });
        $(document).on(`keyup`, `#txtPhone`, function () { SignUp.phone = $(this).val(); });
    })();

    return {
        token: token,
        phone: phone,
        getReCaptcha: getReCaptcha,
        getHtmlCard: getHtmlCard,
        getHtmlCardSuccess: getHtmlCardSuccess,
        signUp: signUp
    }

})();
const ForgotPassword = (function () {

    //Private -------------------------------------
    
    const _forgotPassword = function () {
        
        const vm = {
            email: $(`#txtEmail`).val()
        };
        
        try {
        
            Validation.getIsValidForm($('m-card'));
        
            console.log(vm);
            Global.post('Member_ForgotPassword', vm)
                .done(function (data) {
                    Validation.done(data);
                    Validation.notification(0, `Success`, `Please check your email inbox.`, `success`);
                    Module.replaceCard(`Forgot Password`, `SignIn.getHtmlCard`, ``);
                })
                .fail(function (data) {
                    Validation.fail(data);
                    if (data.responseJSON.Message == `Member does not exist, please sign up to continue.`)
                        Module.replaceCard(`Forgot Password`, `SignUp.getHtmlCard`, ``);
                });
        
        } catch (ex) {
            Validation.fail(ex);
        }

    }
    const _resetForgotPassword = function () {
        
        const vm = {
            email: Application.getUrlParameter('email'),
            password: $('#txtPassword').val(),
            forgotPasswordToken: Application.getUrlParameter('forgotPasswordToken')
        }
        
        try {
        
            Validation.getIsValidForm($('m-card'));
        
            Global.post('Member_EditResetPassword', vm)
                .done(function (data) {
                    Validation.done(data);
                    window.location.href = `${Application.baseUrl}?email=${vm.email}&success=${encodeURI(`Your password has been successfully reset.`)}`;
                })
                .fail(function (data) {
                    Validation.fail(data);
                    if (data.responseJSON.Message == `Token expired. Please request another reset password.`)
                        Module.replaceCard(`Reset Password`, `ForgotPassword.getHtmlCard`, ``);
                });
            
        } catch (ex) {
            Validation.fail(ex);
        }

    }

    //Public -------------------------------------
    
    const getHtmlCard = function () {
        Application.type = `ForgotPassword.getHtmlCard`;
        return `

            <m-card class="d1" tabindex="0" role="region" data-label="Forgot Password">
                <m-flex data-type="row" class="sC">

                    <m-image class="icon mR tR" style="background-image: url('${Settings.logo}');">
                    </m-image>

                    <m-flex data-type="col" class="n">
                        <h1>${Settings.name}</h1>
                        <h2>Forgot Password</h2>
                    </m-flex>

                </m-flex>
                <m-flex data-type="col">

                    <m-input>
                        <label for="txtEmail">Email</label>
                        <input type="text" id="txtEmail" placeholder="Email" required value="${SignIn.email}" />
                    </m-input>

                </m-flex>
                <m-flex data-type="row" class="footer">

                    <m-button data-type="secondary" class="btnReplaceCard" data-label="Forgot Password" data-function="SignIn.getHtmlCard" tabindex="0" role="button" data-label="Cancel">
                        Cancel
                    </m-button>
                    <m-button data-type="primary" id="btnForgotPassword" tabindex="0" role="button" data-label="Reset Password">
                        Send Email
                    </m-button>

                </m-flex>
            </m-card>

            `;
    }
    const getHtmlCardReset = function () {
        Application.type = `ForgotPassword.getHtmlCardReset`;
        return `

            <m-card class="d1" tabindex="0" role="region" data-label="Reset Password">
                <m-flex data-type="row" class="sC">

                    <m-image class="icon mR tR" style="background-image: url('${Settings.logo}');">
                    </m-image>

                    <m-flex data-type="col" class="n">
                        <h1>${Settings.name}</h1>
                        <h2>Reset Password</h2>
                    </m-flex>

                </m-flex>
                <m-flex data-type="col">

                    <h3>${Application.getUrlParameter(`email`)}</h3>

                    <m-input>
                        <label for="txtPassword">New Password</label>
                        <input type="password" id="txtPassword" placeholder="New Password" required />
                    </m-input>

                    <m-input>
                        <label for="txtVerifyPassword">Verify New Password</label>
                        <input type="password" id="txtVerifyPassword" placeholder="Verify New Password" required />
                    </m-input>

                </m-flex>
                <m-flex data-type="row" class="footer">

                    <m-button data-type="primary" id="btnResetPassword" tabindex="0" role="button" data-label="Reset Password">
                        Reset Password
                    </m-button>

                </m-flex>
            </m-card>

            `;
    }
    
    const forgotPassword = function () {
        _forgotPassword();
    }
    const resetForgotPassword = function () {
        _resetForgotPassword();
    }

    const _init = (function () {
        $(document).on(`tap`, `#btnForgotPassword`, function () { _forgotPassword(); });
        $(document).on(`tap`, `#btnResetPassword`, function () { _resetForgotPassword(); });
    })();

    return {
        getHtmlCard: getHtmlCard,
        getHtmlCardReset: getHtmlCardReset,
        forgotPassword: forgotPassword,
        resetForgotPassword: resetForgotPassword
    }

})();

const Company = (function () {

    //Private ---------------------------------------------
    let _timeout;
    let _arr;

    const _add = function () {

        Company.vm.companyId = Global.guidEmpty;
        Company.vm.name = $(`#txtCompany`).val();
        Company.vm.email = $(`#txtEmail`).val();

        _reset();

    }
    const _addSelect = function (id) {
        Company.vm = _arr.filter(function (obj) { return obj.companyId == id; })[0];
        _reset();
    }

    const _delete = function () {

        $(`#txtCompany`).val(``); //Company.vm.name
        $(`.lstCompanies m-card`).remove();

        Company.vm.companyId = Global.guidEmpty;
        Company.vm.name = ``;
        Company.vm.email = ``;

    }

    const _getCompanySignUp = function (value) {
        
        clearTimeout(_timeout);
        _timeout = setTimeout(function () {
        
            const vm = {
                search: value
            }
            
            Global.post(`Company_GetSignUp`, vm)
                .done(function (data) {
                    _arr = data;
                    $(`#txtCompany`).parent().find(`.absolute`).remove();
                    $(`#txtCompany`).parent().append(_getHtmlBodyList(data));
                })
                .fail(function (data) {
                    Validation.notification(2);
                });
        
        }, Global.keyUpTimeout);

    }

    const _getHtmlBodyList = function (arr) {
        
        let html = ``;

        for (let obj of arr)
            html += Company.getHtmlCard(obj, true);
        
        return `

            <m-flex data-type="col" class="n absolute w d2">
                <m-flex data-type="col" class="n list selectable">
                    <h2>Select an existing company or create a new company.</h2>
                    ${html}
                </m-flex>
                <m-flex data-type="row" class="footer">
                    <m-button data-type="secondary" class="btnCloseCompany">
                        Cancel
                    </m-button>
                </m-flex>
            </m-flex>
    
            `;
    }

    const _close = function () {
        $(`#txtCompany`).parent().find(`.absolute`).remove();
    }
    const _reset = function () {
        $(`#txtCompany`).val(``);
        $(`#txtCompany`).parent().find(`.absolute`).remove();
        $(`.lstCompanies m-card`).remove();
        $(`.lstCompanies`).append(Company.getHtmlCard(Company.vm, false));
    }

    //Public ----------------------------------------------
    const constructor = function (companyId, printerId, prayerId, name, email, fontHeader, fontSubHeader, fontBody, phone, website,
        billingAddressLine1, billingAddressLine2, billingCity, billingState, billingZip,
        shippingAddressLine1, shippingAddressLine2, shippingCity, shippingState, shippingZip, isApproved, isDeleted) {
        this.companyId = companyId;
        this.printerId = printerId;
        this.prayerId = prayerId;
        this.name = name;
        this.email = email;
        this.fontHeader = fontHeader;
        this.fontSubHeader = fontSubHeader;
        this.fontBody = fontBody;
        this.phone = phone;
        this.website = website;
        this.billingAddressLine1 = billingAddressLine1;
        this.billingAddressLine2 = billingAddressLine2;
        this.billingCity = billingCity;
        this.billingState = billingState;
        this.billingZip = billingZip;
        this.shippingAddressLine1 = shippingAddressLine1;
        this.shippingAddressLine2 = shippingAddressLine2;
        this.shippingCity = shippingCity;
        this.shippingState = shippingState;
        this.shippingZip = shippingZip;
        this.isApproved = isApproved;
        this.isDeleted = isDeleted;
        this.companyLocations = [];
        this.companyTemplates = [];
        this.items = [];
    }
    let vm = new constructor(Global.guidEmpty, Global.guidEmpty, Global.guidEmpty, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, ``, false, false);
    
    const getHtmlCard = function (obj, isAdd) {
        return `

            <m-card class="btn${(isAdd) ? `Select` : `Delete`}Company" data-id="${obj.companyId}">
                <m-flex data-type="row">
                    <m-flex data-type="col" class="n">
                        <h1>${obj.name}</h1>
                        <h2>${obj.email}</h2>
                    </m-flex>
                    ${(isAdd) ? `
                    <m-button data-type="primary" class="" data-id="">
                        Select
                    </m-button>` : `
                    <m-button data-type="secondary" class="" data-id="">
                        Remove
                    </m-button>`}
                </m-flex>
            </m-card>
                
            `;
    }

    const _init = (function () {
        $(document).on(`tap`, '.btnAddCompany', function () { _add(); });
        $(document).on(`tap`, '.btnSelectCompany', function () { _addSelect($(this).attr(`data-id`)); });
        $(document).on(`tap`, '.btnDeleteCompany', function () { _delete($(this).attr(`data-id`)); });
        $(document).on(`tap`, '.btnCloseCompany', function () { _close(); });
        $(document).on(`keyup`, '#txtCompany', function () { _getCompanySignUp($(this).val()); });
    })();

    return {
        vm: vm,
        constructor: constructor,
        getHtmlCard: getHtmlCard
    }

})();

function callBack(token) {
    SignUp.getReCaptcha(token);
}
function onSignIn(googleUser) {
    SignIn.signInGoogle(googleUser);
}
function signOut() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        console.log('User signed out.');
    });
}
Application.init();