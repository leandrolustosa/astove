(function () {
    angular.module('AstoveApp', [
        'ngResource',
        'ngSanitize',
        'ngAnimate',
        'ui.bootstrap',
        'ui.sortable',
        'ui.router',

        'ngTouch',
        'toastr',
        'smart-table',
        "xeditable",
        'ui.slimscroll',
        'angular-progress-button-styles',  

        'datePicker',
        'ngFileUpload',
        'ngImgCrop',
        'ui.mask',
        'localytics.directives', // Chosen
        
        'astove.modal',

        'BlurAdmin.theme'
    ])
})();

angular.module('AstoveApp').factory('HttpErrorInterceptorModule', ["$q", "$rootScope", "$location", "$window",
        function ($q, $rootScope, $location, $window) {
            return {
                // optional method
                'request': function (config) {
                    // do something on success
                    var canceller = $q.defer();
                    var url = $location.protocol() + '://' + $location.host() + (($location.port() == 80) ? '' : ':' + $location.port()) + '/#/';
                    if (typeof config !== 'undefined') {
                        var routeUrl = config.url.toLowerCase();
                        if (routeUrl.indexOf('/api/') == -1 && routeUrl.indexOf('/common/') == -1 && routeUrl.indexOf('/account/') == -1) {
                            if (routeUrl.indexOf('/home') >= 0) {
                                if ($window.location.href.indexOf(url) == -1) {
                                    $window.location.href = '/#/indicacoes' + routeUrl.replace('/home', '').toLowerCase();
                                    config.timeout = canceller.promise;
                                    return config;
                                }
                            }
                            else if (routeUrl.indexOf('/configuracao') >= 0) {
                                if ($window.location.href.indexOf('/configuracao') == -1) {
                                    $window.location.href = '/configuracao#/cfg' + routeUrl.replace('/configuracao', '').toLowerCase();
                                    config.timeout = canceller.promise;
                                    return config;
                                }
                            }
                        }
                    }
                    
                    return config;                    
                },
                'responseError': function (response) {
                    // do something on error
                    if (response.status === 401) {
                        $window.location.href = '/Account/Login';
                    }
                    return $q.reject(response);
                }
            };
        }]).config(["$httpProvider",
            function ($httpProvider) {
                $httpProvider.interceptors.push("HttpErrorInterceptorModule");
            }
        ]);