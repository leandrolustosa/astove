(function () {
    "use strict";
    
    angular.module('AstoveApp').controller('WindowDialogCtrl', ['$scope', 'AstoveService', 'AstoveData', 
        function ($scope, $service, $data) {
            $scope.service = $service;
            $scope.data = $data;

            window.onResult = function (data) {
                $scope.$apply(function () {
                    if ($service.dropdownOptions !== null) {
                        var item = {
                            key: data.id,
                            value: data[$service.dropdownOptions.displayText]
                        }
                        $service.dropdownOptions.items.push(item);
                        $data.model[$service.dropdownOptions.displayValue] = item.key;
                        console.log(item.key);
                        console.log(item.value);
                    }

                    $service.closeDialog();
                });
            };
        } ]);

})();