(function () {
    "use strict";
    
    angular.module('AstoveApp').service('messageBoxService', ['AstoveData', 
        function ($data) {
            this.closed = false;
            this.propertyName = '';

            this.listen = function ($scope) {
                var self = this;
                $scope.$watch('messageBoxService.closed', function (newVal, oldVal) {
                    if (newVal !== oldVal) {
                        var imagemUrl = $data.entity[self.propertyName];
                        var regex = /\?\d+/g;
                        var result = regex.exec(imagemUrl);
                        if (result !== null)
                            imagemUrl = imagemUrl.replace(regex, '') + '?' + new Date().getTime();
                        else
                            imagemUrl = imagemUrl + '?' + new Date().getTime();

                        console.log(imagemUrl);
                        $data.entity[self.propertyName] = imagemUrl;
                    }
                    $scope.loadingData = false;
                }, true);
            };
        } ]);

})();