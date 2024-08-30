'use strict';
UnionSurvey.WithdrawRequest = (function () {

    function _init(tnxId) {
        StartRequestForWithdraw(tnxId);
    }

    function StartRequestForWithdraw(tnxId) {

        $('#withdraw').on('click', function () {

            var tnxId = "";
            let data = { "tnxId": tnxId };

            $.ajax({
                type: "POST",
                url: "/Financial/ValidateSignature",
                data: data,
                headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },

                success: function (result) {

                    if (result.success == "false" || result.success == false) {
                        window.location.reload();

                    } else  if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {

                        UserCode = result.data.userCode; //userCode
                        UserName = result.data.userName;
                        Nonce = result.data.nonce;
                        ExpireTime = result.data.expireTime;
                        Signature = result.data.signature;
                        TransactionId = result.data.transactionId;
                        WithdrawAmount = result.data.amount;

                        $('#withdrawdiv').remove();
                        $('#withdrawroot').show();
                        //NolerNotify.success(result.message);
                        // var divId = "#sur_" + id;
                        // $(divId).remove();
                    }
                    //$('#withdrawfund').removeClass('refresh-active');
                }
            });

        });

    }


    return {
        Init: _init
    };
})();