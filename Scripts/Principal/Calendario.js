$(document).ready(Inicio());

function Inicio() {

    $('#calendario_1').daterangepicker({
        singleDatePicker: true,
        timePicker: true,
        singleClasses: "picker_4",
        timePicker24Hour: true,
        timePickerSeconds: true,
        maxDate: moment().endOf("day"),
        locale: {
            format: 'DD/MM/YYYY HH:mm:ss'
        }
    });


    $('#calendario_2').daterangepicker({
        singleDatePicker: true,
        singleClasses: "picker_4",
        timePicker: false,
        timePicker24Hour: false,
        timePickerSeconds: false,
        maxDate: moment().endOf("day"),
        locale: {
            format: 'DD/MM/YYYY'
        }
    });


}



function getFecha(id_calendario) {

}