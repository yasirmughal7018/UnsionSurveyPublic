'use strict';
UnionSurvey.Question = (function () {


    function _init() {
        Delete();
        GetById();
    }

    function GetById() {
        $('#question-table tbody').on('click', '.fa-edit', function () {

            //$('#tblQuestions').addClass('refresh-active');

            var tr = $(this).closest('tr');
            var id = $(tr).prop('id');
            //var row = table.row(tr);
            //var rowData = row.data();

            //rowData.DT_RowId

            let question = { "questionId": id };

            $.ajax({
                type: "GET",
                url: "/Question/GetById",
                data: question,
                success: function (result) {

                    if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {
                        $('#QuestionId').val(id);
                        $('#Question').val(result.question.question);
                        $('#Option1').val(result.question.option1);
                        $('#Option2').val(result.question.option2);
                        $('#Option3').val(result.question.option3);
                        $('#Option4').val(result.question.option4);
                    }
                    //$('#tblQuestions').removeClass('refresh-active');

                }
            });
        });
    }
    function Delete() {
        $('#question-table tbody').on('click', '.fa-trash-alt', function () {

            //$('#tblQuestions').addClass('refresh-active');

            let tr = $(this).closest('tr');
            let id = $(tr).prop('id');

            //rowData.DT_RowId
            let question = { "questionId": id };

            $.ajax({
                type: "DELETE",
                url: "/Question/Delete",
                data: question,
                headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },
                success: function (result) {

                    if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {
                        $(tr).remove();
                    }
                    //$('#tblQuestions').removeClass('refresh-active');

                }
            });
        });
    }


    return {
        Init: _init
    };
})();