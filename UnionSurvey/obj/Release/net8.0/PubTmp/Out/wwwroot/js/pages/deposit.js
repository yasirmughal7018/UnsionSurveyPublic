'use strict';
UnionSurvey.Deposit = (function () {

    function _init() {
        ClickAmountButton();
        StartDeposit();
    }

    function ClickAmountButton() {
        $('.yam').on('click', function () {
            $('#Amount').val($(this).text() * 1);
        });
    }


    function StartDeposit() {

        $('#deposit').on('click', function () {
            let amount = $('#Amount').val() * 1;

            if (amount == 0) {
                $('#error').show();
            } else {
                $('#error').hide();

                // $('#depositfund').addClass('refresh-active');

                let tnxAmount = { "amount": amount };

                $.ajax({
                    type: "POST",
                    url: "/Financial/SaveDeposit",
                    data: tnxAmount,
                    headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },
                    success: function (result) {

                        if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {

                            depositAmount = amount; ///**1e6*/ deposite Amount and change in two area.
                            tnxId = result.data.transactionId; //13455
                            $('#root').show();
                            $('#depositDiv').remove();
                        }
                        // $('#depositfund').removeClass('refresh-active');
                    }
                });
            }
        });


    }


    return {
        Init: _init
    };
})();