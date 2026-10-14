
(function ($) {
    'use strict';

    const educationalQualificationMap = {
        underDiploma: 'زیردیپلم',
        diploma: 'دیپلم',
        postDiploma: 'فوق دیپلم',
        bachelor: 'لیسانس',
        master: 'فوق لیسانس',
        doctor: 'دکتری',
        postDoctor: 'فوق دکتری',
        hozavi: 'حوزوی',
        Unknow: 'نامشخص'
    };

    const genderMap = {
        male: 'مرد',
        female: 'زن'
    };

    const maritalStatusMap = {
        single: 'مجرد',
        married: 'متاهل',
        divorced: 'مطلقه',
        widowed: 'بیوه'
    };

    const managementLayerMap = {
        noManager: 'ندارد',
        layer1: 'لایه یک',
        layer2: 'لایه دو',
        layer3: 'لایه سه',
        layer4: 'لایه چهار',
        layer5: 'لایه پنج',
        layer6: 'لایه شش'
    };

    $.extend({
        convertEducationalQualificationToFarsi: function (input) {
            return educationalQualificationMap[input] ?? '';
        }
    });

    $.extend({
        convertGenderToFarsi: function (input) {
            return genderMap[input] ?? '';
        }
    });

    $.extend({
        convertMaritalStatusToFarsi: function (input) {
            return maritalStatusMap[input] ?? '';
        }
    });

    $.extend({
        convertManagementLayerToFarsi: function (input) {
            return managementLayerMap[input] ?? '';
        }
    });

})(jQuery);
