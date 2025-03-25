// Shared variables
var table;
var datatable;
var updatedRow;
var exportedCols = [];

function showSuccessMessage(message = 'Saved Successfully!') {
    //Use Sweet Alert
    Swal.fire({
        icon: 'success',
        title: 'Good Job',
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}

function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}

function onModalBegin() {
    $('body :submit').prop('disabled', true).attr("data-kt-indicator", "on");
}

function onModalSuccess(row) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (typeof updatedRow !== 'undefined' && updatedRow !== null) {
        datatable.row(updatedRow).data($(row).children().map(function () {
            return $(this).html();
        }).get()).draw(false); // Update row without full table refresh
        updatedRow = undefined;
    } else {
        datatable.row.add($(row)).draw(false); // Add new row without refreshing
    }
    // Reinitialize KTMenu only if needed
    KTMenu.init();
    KTMenu.initHandlers()

}

function onModalComplete() {
    $('body :submit').prop('disabled', false).removeAttr("data-kt-indicator");

}

//Data Table
var headers = $('th');
$.each(headers, function (i) {
    var col = $(this);
    if (!col.hasClass('js-no-export'))
        exportedCols.push(i);


});

// Class definition
var KTDatatables = function () {

    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            'pageLength': 10,
        });
    }

    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $('.js-datatables').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Search Datatable
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

$(document).ready(function () {
    //SweetAlert
    var messageElement = $('#Message');
    if (messageElement.length && messageElement.text().trim() !== '') {
        showSuccessMessage(messageElement.text());
    }
    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    //Handle bootsstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');
        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);

            },
            error: function () {
                showErrorMessage();
            }
        });
        modal.modal('show');
    });

    //Handel Toggle Status
    $(document).ready(
        function () {
            $('body').delegate('.js-toggle-status', 'click',
                function () {
                    var btn = $(this);

                    bootbox.confirm({
                        message: "Are you sure that you need to toggle this item status?",
                        buttons: {
                            confirm: {
                                label: 'Yes',
                                className: 'btn-danger'
                            },
                            cancel: {
                                label: 'No',
                                className: 'btn-secondary'
                            }
                        },
                        callback: function (result) {
                            if (result) {
                                $.post({
                                    url: btn.data('url'),
                                    data: {
                                        '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                                    },
                                    success: function (LastUpdatedOn) {
                                        var row = btn.closest('tr'); // More optimized than parents('tr')
                                        var status = row.find('.js-status');
                                        var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';

                                        // Update status text and styles
                                        status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                                        row.find('.js-updated-on').html(LastUpdatedOn);
                                        row.addClass('animate__animated animate__flash');

                                        //Use Sweet Alert From site.js
                                        showSuccessMessage("Edit Successfully");

                                    },
                                    error: function () {
                                        //Use Sweet Alert From site.js
                                        showErrorMessage();

                                    }
                                });
                            }
                        }
                    });
                });
        }
    );
});