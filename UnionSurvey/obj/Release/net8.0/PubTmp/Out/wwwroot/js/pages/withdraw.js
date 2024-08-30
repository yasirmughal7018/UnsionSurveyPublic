'use strict';
UnionSurvey.Withdraw = (function () {

    function _init() {
        ClickAmountButton();
        StartRequestForWithdraw();
    }

    function ClickAmountButton() {
        $('.yam').on('click', function () {
            var availableBalance = $('#AvailableBalance').val() * 1;

            $('#Amount').val(availableBalance | 0);
        });
    }


    function StartRequestForWithdraw() {

        $('#withdraw').on('click', function () {
            let amount = $('#Amount').val() * 1;
            var availableBalance = $('#AvailableBalance').val() * 1;

            if (amount == 0 || amount > availableBalance) {
                $('#error')
                    .text('Please enter correct amount.')
                    .show();
            } else {
                $('#error').hide();

                //$('#withdrawfund').addClass('refresh-active');

                let data = { "amount": amount };

                $.ajax({
                    type: "POST",
                    url: "/Financial/SaveWithDraw",
                    data: data,
                    headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },
                    success: function (result) {
                        debugger;
                        if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {

                            if (result.success === false || result.success === 'false')
                                $('#error')
                                    .text(result.message)
                                    .show();
                            else
                                window.location.reload();
                            //NolerNotify.success(result.message);
                            // var divId = "#sur_" + id;
                            // $(divId).remove();
                        }
                        //$('#withdrawfund').removeClass('refresh-active');
                    }
                });
            }
        });

    }


    return {
        Init: _init
    };
})();