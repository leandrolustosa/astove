(function () {
    "use strict";

    angular.module('AstoveApp').controller('ConfigurationCtrl', ['$scope', 'AstoveService', 'AstoveData', 'AstoveDataList', 'AstoveCommon',
            function ($scope, $service, $data, $datalist, $global) {
                $scope.data = $data;
                $scope.global = $global;
                
                $scope.setSkinClass = function (skinClass) {
                    window.localStorage.setItem('skinClass', skinClass);
                    $global.skinClass = skinClass;
                };

                $scope.setCollapseMenu = function () {
                    var collapseMenuClass = ($global.collapseMenu) ? 'mini-navbar' : '';
                    window.localStorage.setItem('collapseMenu', $global.collapseMenu);
                    window.localStorage.setItem('collapseMenuClass', collapseMenuClass);
                    $global.collapseMenuClass = collapseMenuClass;
                };
                 
            }]);
})();