(function () {
    "use strict";

    var loginApp = angular.module('loginApp',
        [
            'directives'
        ]);

    loginApp.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
    }]);
})();