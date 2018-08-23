(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('ImageEditorCtrl', ['$scope', '$routeParams', '$location', '$http', 'AstoveData', 'AstoveCommon',
        function ($scope, $routeParams, $location, $http, $data, $global) {
            $global.loading = true;
            $scope.global = $global;
            $scope.origin = $location.protocol() + "//" + $location.host() + ($location.port() !== 80 ? ':' + $location.port() : '');

            $scope.image = {
                path: "",
                width: 0,
                height: 0
            };

            $scope.loadImage = function () {
                var img = new Image();
                img.onload = function () {
                    $scope.$apply(function () {
                        var fator = 1;
                        if (img.width >= 845)
                            fator = 845 / img.width;
                        $scope.image.width = img.width * fator;
                        $scope.image.height = img.height * fator;
                        $scope.largura = img.width * fator;
                        $scope.altura = img.height * fator;
                        $scope.image.path = $scope.imagemPath;
                    });
                };
                img.src = $scope.imagemPath;
                $global.loading = false;
            };

            $scope.imagemUrl = $routeParams.imagemUrl.replace(/([$])/g, '/');
            $scope.imagemPath = $scope.imagemUrl;
            $scope.larguraAlvo = parseInt($routeParams.largura);
            $scope.alturaAlvo = parseInt($routeParams.altura);
            $scope.larguraMin = (typeof $routeParams.larguraMin === 'undefined' || $routeParams.larguraMin === null) ? null : parseInt($routeParams.larguraMin);
            $scope.alturaMin = (typeof $routeParams.alturaMin === 'undefined' || $routeParams.alturaMin === null) ? null : parseInt($routeParams.alturaMin);
            $scope.largura = 0;
            $scope.altura = 0;
            $scope.razao = ($routeParams.razao === null) ? null : parseFloat($routeParams.razao);

            $scope.redimensionar = function (width, height, actionName) {
                $global.loading = true;
                $http.post(typeof actionName == 'undefined' ? '/comum/redimensionar' : actionName, { imagemUrl: $scope.imagemUrl, largura: width, altura: height, larguraMin: (($scope.larguraMin === null) ? $scope.larguraAlvo : $scope.larguraMin), alturaMin: (($scope.alturaMin === null) ? $scope.alturaAlvo : $scope.alturaMin) })
                        .success(function (data, status, headers, config) {
                            $scope.imagemPath = data.Path;
                            $scope.loadImage();
                            $global.loading = false;
                            var $parentScope = parent.angular.element('#ImageEditorDialogCtrl').scope();
                            $parentScope.$apply(function () {
                                $parentScope.service.closeDialog();
                            });
                        })
                        .error(function (responseData) {
                            $scope.largura = $scope.image.width;
                            $scope.altura = $scope.image.height;

                            showValidationSummaryMetro('Erro', 'OK', errResponse.data);
                            $global.loading = false;
                        });
            };

            $scope.recortar = function (actionName) {
                $global.loading = true;
                $http.post(typeof actionName === 'undefined' ? '/comum/recortar' : actionName, { imagemUrl: $scope.imagemPath.replace(/\?\d+/g, ''), larguraOriginal: $scope.largura, alturaOriginal: $scope.altura, w: $scope.coords.w, h: $scope.coords.h, x: $scope.coords.x, y: $scope.coords.y })
                        .success(function (data, status, headers, config) {
                            $scope.imagemPath = data.Path;
                            $scope.loadImage();
                            $global.loading = false;
                            var $parentScope = parent.angular.element('#ImageEditorDialogCtrl').scope();
                            $parentScope.$apply(function () {
                                $parentScope.service.closeDialog();
                            });
                        })
                        .error(function (errResponse) {
                            showValidationSummaryMetro('Erro', 'OK', errResponse.data);
                            $global.loading = false;
                        });
            };

            $scope.options = {
                aspectRatio: $scope.larguraAlvo / $scope.alturaAlvo,
                minSize: [($scope.larguraMin === null) ? $scope.larguraAlvo : $scope.larguraMin, ($scope.alturaMin === null) ? $scope.alturaAlvo : $scope.alturaMin],
                maxSize: [$scope.larguraAlvo * 3, $scope.alturaAlvo * 3]
            };

            $scope.selected = function (c) {
                $scope.coords = c;
            };

            $scope.loadImage();
        } ]);

})();