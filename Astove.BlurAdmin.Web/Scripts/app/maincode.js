(function () {
    "use strict";

    var codeApp = angular.module('codeApp', ['directives', 'ui.codemirror', 'ngSanitize']);
    codeApp.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
    }]);
})();