/// <reference path="../../bower_components/underscore/underscore.js" />
/// <reference path="../../bower_components/angular/angular.js" />

(function () {
    "use strict";
    
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function(obj, start) {
             for (var i = (start || 0), j = this.length; i < j; i++) {
                 if (this[i] === obj) { return i; }
             }
             return -1;
        }
    }

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };
    
    angular.module('AstoveApp').service('AstoveService', ['AstoveApi', 'AstoveData', 'AstoveDataList', 'AstoveCommon', 'AstoveError', '$location', '$q', '$window', '$timeout', '$http',
            function ($api, $data, $datalist, $global, $error, $location, $q, $window, $timeout, $http) {
                this.showDialog = false;
                this.iframeUrl = '';
                this.dropdownOptions = {};

                this.actions = [];
                this.actions.push({ path: 'list', action: 'list'});
                this.actions.push({ path: 'details', action: 'details'});
                this.actions.push({ path: 'step', action: 'details'});
                this.actions.push({ path: 'update', action: 'update'});
                this.actions.push({ path: 'insert', action: 'insert'});
                this.actions.push({ path: 'imageeditor', action: 'imageeditor'});

                this.EMAIL_REGEXP = /^[a-z0-9!#$%&'*+/=?^_`{|}~.-]+@[a-z0-9-]+\.([a-z0-9]{2,3})+(\.[a-z0-9-]{2})*$/i;
                
                this.openDialog = function($scope, url, dropdownList) {
                    this.iframeUrl = url;
                    this.showDialog = true;
                    this.dropdownOptions = dropdownList;
                };

                this.closeDialog = function () {
                    this.iframeUrl = '';
                    this.dropdownOptions = {};
                    this.showDialog = false;
                };

                this.formIsValid = function(form) {
                    return (typeof form !== 'undefined' && form !== null && form.$valid) || (typeof form === 'undefined' || form === null);
                };

                this.redirect = function (url) {
                    $location.path(url);
                };

                this.onChanged = function($scope, action, properties) {
                    var self = this;
                    if (action==='redirect') {
                        if (typeof onRedirect === 'function') {
                            onRedirect(($scope.loadModel) ? $data.model : $data.entity);
                        }
                        else {
                            self.redirect(properties);
                        }
                    }
                    else if (action==='refresh' && typeof properties !== 'undefined') {
                        angular.forEach(
                            properties,
                            function(propertyName) {
                                self.reloadData($scope, propertyName);
                            });
                    }
                    else if (action === 'refresh' && typeof properties === 'undefined') {
                        self.reloadDataList($scope);
                    }
                    else if (action === 'refreshEntity' && typeof properties !== 'undefined') {
                        var id = properties;
                        self.getData($scope, id, 'select');
                    }
                    else if (action === 'refreshModel' && typeof properties !== 'undefined') {
                        var id = properties;
                        self.getModel($scope.entityNameUpdate, id);
                    }
                    else if (action === 'refreshOptions' && typeof properties !== 'undefined') {
                        var propertyKey = properties.key;
                        var propertyOptions = properties.options;
                        var propertyDisplay = properties.display;
                        
                        $data.model[propertyOptions]['items'].push({ key: $data.modalModel.id, value: $data.modalModel[propertyDisplay] });
                        _.sortBy($data.model[propertyOptions]['items'], function (o) { return o.value; });
                        $data.model[propertyKey] = $data.modalModel.id;
                    }
                    else if (action === 'message' && typeof properties !== 'undefined') {
                        alert(properties);
                    }
                    else if (action==='route' && typeof properties !== 'undefined') {
                        var obj = ($scope.loadModel === 'true') ? $data.model : $data.entity;
                        if (properties.indexOf('list') != -1 || properties.indexOf('insert') != -1 || properties.indexOf('register') != -1)
                            $location.path('/' + properties);
                        else if (properties==='details' || properties==='step' || properties==='update' || properties==='success'|| properties==='statement' || properties==='option' || properties==='profile')
                            $location.path('/' + properties + '/' + obj.id);
                    }                    
                    else if (action === 'callback') {
                        $scope.callbackResult(properties);
                    }
                };

                this.getOrigin = function() {
                    return $location.protocol() + "//" + $location.host() + ($location.port()!=80 ? ':' + $location.port() : '');
                };
            
                this.getAction = function() {
                    var path = $location.getPath().substr(1);
                    if (path.indexOf('/') !== -1)
                        path = path.substr(0, path.indexOf('/'));

                    var action = '';
                    for (var i=0; i<this.actions.length; i++) {
                        if (this.actions[i].path===path) {
                            action = this.actions[i].action;
                            break;
                        }
                    }

                    return action;
                };

                this.getInsertModel = function (controllerName, args, append) {
                    $global.loading = true;
                    var parameters = { page: 1, take: 1, type: 'InsertModel', args: args };
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        if (typeof append !== 'undefined')
                            _.extend(parameters, append);
                        $data.model = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);                        
                    });
                };

                this.getInsertModelAsync = function (controllerName, args, append) {
                    $global.loading = true;
                    var parameters = { page: 1, take: 1, type: 'InsertModel', args: args };
                    if (typeof append !== 'undefined')
                        _.extend(parameters, append);
                    var deferred = $q.defer();
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        $global.loading = false;
                        $data.model = resp;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.getInsertModalModel = function (controllerName, args, append) {
                    $global.loading = true;
                    var parameters = { page: 1, take: 1, type: 'InsertModel', args: args };
                    if (typeof append !== 'undefined')
                        _.extend(parameters, append);
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        $data.modalModel = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getInsertModalModelAsync = function (controllerName, args, append) {
                    var deferred = $q.defer();
                    var parameters = { page: 1, take: 1, type: 'InsertModel', args: args };
                    if (typeof append !== 'undefined')
                        _.extend(parameters, append);
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };
		
				this.getInsertModelAsync = function (controllerName, id) {
                    var deferred = $q.defer();
                    $api(controllerName).getAll({ page: 1, take: 1, type: 'InsertModel' }).$promise.then(function (resp) {
                        $data.model = resp;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse.data);
                    });
                    return deferred.promise;
                };

                this.getModel = function (controllerName, id) {
				    $global.loading = true;
                    $api(controllerName).getAll({ page: 1, take: 1, entityId: id, type: 'UpdateModel' }).$promise.then(function (resp) {
                        $data.model = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getUpdateModel = function (controllerName, args, append) {
                    $global.loading = true;
                    var parameters = { page: 1, take: 1, type: 'UpdateModel', args: args };
                    if (typeof append !== 'undefined')
                        parameters = _.extend(parameters, append);
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        $data.model = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getUpdateModelAsync = function (controllerName, args, append) {
                    $global.loading = true;
                    var deferred = $q.defer();
                    var parameters = { page: 1, take: 1, type: 'UpdateModel', args: args };
                    if (typeof append !== 'undefined')
                        parameters = _.extend(parameters, append);
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        $data.model = resp;
                        $global.loading = false;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.getUpdateModalModel = function (controllerName, args, append) {
                    $global.loading = true;
                    var parameters = { page: 1, take: 1, type: 'UpdateModel', args: args };
                    if (typeof append !== 'undefined')
                        _.extend(parameters, append);
                    $api(controllerName).getAll(parameters).$promise.then(function (resp) {
                        $data.modalModel = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getModelAsync = function (controllerName, id) {
                    $global.loading = true;
                    var deferred = $q.defer();
                    $api(controllerName).getAll({ page: 1, take: 1, entityId: id, type: 'UpdateModel' }).$promise.then(function (resp) {
                        $global.loading = false;
                        $data.model = resp;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.getDataModel = function (controllerName, id) {
                    $global.loading = true;
                    $api(controllerName).get({ id: id }).$promise.then(function (resp) {
                        $data.model = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getDataModelAsync = function (controllerName, id) {
                    var deferred = $q.defer();
                    $global.loading = true;
                    $api(controllerName).get({ id: id }).$promise.then(function (resp) {
                        $global.loading = false;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.getDataActionAsync = function (controllerName, args) {
                    var params = _.map(args, function (value, key) { return key + '=' + value.toString() });
                    var deferred = $q.defer();
                    $global.loading = true;
                    $api(controllerName + '?' + params).getAction().$promise.then(function (resp) {
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.getDataModalModel = function (controllerName, id) {
                    $global.loading = true;
                    $api(controllerName).get({ id: id }).$promise.then(function (resp) {
                        $data.modalModel = resp;
                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.search = function (arg) {
                    var deferred = $q.defer();
                    var controllerName = 'search/getsearchs';
                    $api(controllerName).get({ text: arg }).$promise.then(function (data) {
                        if (data.isValid)
                            deferred.resolve(data.items);
                        else
                            deferred.resolve([]);
                    }, function (errResponse) {
                        $error.show(errResponse);
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.insert = function($scope, action, properties) {
                    this.addEntity($scope, $scope.entityNameInsert, $data.model, action, properties);
                };

                this.addEntityModalForm = function ($scope, controllerName, entity, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).insert(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        $scope.messagetype = '';
                        $scope.message = '';
                        $scope.showAlert = false;
                        $scope.hasError = false;
                        $scope.success = false;
                        if (resp.isValid === true) {
                            $data.modalModel.id = resp.id;
                            $scope.ok();
                            self.onChanged($scope, action, (typeof properties !== 'undefined') ? properties : resp);
                        }
                        else {
                            $scope.message = resp.message;
                            $scope.showAlert = true;
                        }
                    }, function (errResponse) {
                        $global.loading = false;
                        $scope.message = 'Erro inesperado';
                        $scope.showAlert = true;
                        $error.show(errResponse);
                    });
                };

                this.addModalForm = function ($scope, controllerName, entity, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).insert(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        $scope.messagetype = '';
                        $scope.message = '';
                        $scope.showAlert = false;
                        $scope.hasError = false;
                        $scope.success = false;
                        if (resp.isValid === true) {
                            self.onChanged($scope, action, properties);
                        }
                        else {
                            $scope.message = resp.message;
                            $scope.showAlert = true;
                        }
                    }, function (errResponse) {
                        $global.loading = false;
                        $scope.message = 'Erro inesperado';
                        $scope.showAlert = true;
                        $error.show(errResponse);
                    });
                };

                this.addEntityForm = function ($scope, controllerName, entity, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).insert(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        $scope.messagetype = '';
                        $scope.message = '';
                        $scope.showAlert = false;
                        $scope.hasError = false;
                        $scope.success = false;
                        if (resp.isValid === true) {
                            self.onChanged($scope, action, properties);
                        }
                    }, function (errResponse) {
                        $global.loading = false;
                        $scope.message = 'Erro inesperado';
                        $scope.showAlert = true;
                        $error.show(errResponse);
                    });
                };

                this.addEntity = function ($scope, controllerName, entity, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).insert(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        self.onChanged($scope, action, properties);
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.addEntityAsync = function ($scope, controllerName, entity) {
                    var deferred = $q.defer();
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).insert(entity).$promise.then(function (resp) {
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.addEntityToList = function (form, propertyName, listName, entity) {
                    if (this.formIsValid(form)) {
                        $data.entity[propertyName][listName].push(entity);
                    }
                    else {
                        $error.show('Atenção!', 'OK', ['Preencha todos os campos obrigatórios']);
                    }
                };

                this.addEntityToArray = function (propertyName, listName, childName, controllerName) {
                    $data.entity[propertyName][listName].push($data.entity[propertyName][childName]);
                    if (controllerName) {
                        $global.loading = true;
                        $api(controllerName).getAll({ page: 1, take: 1, type: 'InsertModel' }).$promise.then(function (resp) {
                            $data.entity[propertyName][childName] = resp;
                            $global.loading = false;
                        }, function (errResponse) {
                            $global.loading = false;
                            $error.show(errResponse);
                        });
                    }
                };

                this.addEntityToChildArray = function (propertyName, listName, childName, entity) {
                    $data.entity[propertyName][childName][listName].push(entity);
                };

                this.addModelToList = function ($scope, visibleProperty, controllerName, propertyName, propertyList) {
                    $global.loading = true;
                    $api(controllerName + '/' + 'validateInsert').insert($data.model[propertyName]).$promise.then(function (resp) {
                        $timeout(function() {
                            var model = jQuery.extend({},$data.model[propertyName]);
                            $data.model[propertyList].push(model);
                            $data.model[propertyName] = resp;
                            $global.loading = false;
                            $scope[visibleProperty] = false;
                        });
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.update = function ($scope, action, properties) {
                    this.updateEntity($scope, $scope.entityNameUpdate, $data.model, action, properties);
                };

                this.updateEntity = function($scope, controllerName, model, action, properties) {
                    var self = this;
                    $global.loading = true;
            
                    $api(controllerName, $scope.antiForgeryToken).update(model).$promise.then(function (response) {
                        $global.loading = false;
                        if (!Array.isArray(response)) {
                            self.onChanged($scope, action, properties);
                        }
                        else {
                            $global.loading = false;
                            $error.show('Requisição inválida', 'OK', response);  
                        }
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.updateEntityAsync = function($scope, controllerName, model) {
                    var deferred = $q.defer();
                    $global.loading = true;
            
                    $api(controllerName, $scope.antiForgeryToken).update(model).$promise.then(function (response) {
                        $global.loading = false;
                        if (!Array.isArray(response)) {
                            deferred.resolve(response);
                        }
                        else {
                            deferred.reject(response);
                        }
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });

                    return deferred.promise;
                };

                this.updateEntityForm = function ($scope, controllerName, entity, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).update(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        $scope.messagetype = '';
                        $scope.message = '';
                        $scope.showAlert = false;
                        $scope.hasError = false;
                        $scope.success = false;
                        self.onChanged($scope, action, properties);
                    }, function (reason) {
                        $global.loading = false;
                        $scope.message = 'Erro inesperado';
                        $scope.showAlert = true;
                        $error.show(reason);
                    });
                };

                this.updateEntityModalForm = function ($scope, controllerName, entity, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).update(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        $scope.messagetype = '';
                        $scope.message = '';
                        $scope.showAlert = false;
                        $scope.hasError = false;
                        $scope.success = false;                        
                        if (resp.isValid === true) {
                            $scope.ok();
                            self.onChanged($scope, action, properties);
                        }
                        else {
                            $scope.message = resp.message;
                            $scope.showAlert = true;
                        }
                    }, function (errResponse) {
                        $global.loading = false;
                        $scope.message = 'Erro inesperado';
                        $scope.showAlert = true;
                        $error.show(errResponse);
                    });
                };

                this.updateEntityFormAsync = function ($scope, controllerName, entity) {
                    var deferred = $q.defer();
                    var self = this;
                    $global.loading = true;
                    $api(controllerName, $scope.antiForgeryToken).update(entity).$promise.then(function (resp) {
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.executeAction = function (actionName, entity, $scope, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(actionName).insert(entity).$promise.then(function (resp) {
                        $data.actionMessage = 'A ação foi realizada com sucesso!';
                        if (typeof resp.message !== 'undefined' && resp.message !== null && resp.message != '')
                            $data.actionMessage = resp.message;
                        $global.loading = false;
                        self.onChanged($scope, action, properties);
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.executePutAction = function (actionName, entity, $scope, action, properties) {
                    var self = this;
                    $global.loading = true;
                    $api(actionName).update(entity).$promise.then(function (resp) {
                        $data.actionMessage = 'A ação foi realizada com sucesso!';
                        if (typeof resp.message !== 'undefined' && resp.message !== null && resp.message != '')
                            $data.actionMessage = resp.message;
                        $global.loading = false;
                        self.onChanged($scope, action, properties);
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.executeGetAction = function (actionName) {
                    var self = this;
                    $global.loading = true;

                    $api(actionName).getAction().$promise.then(function (resp) {
                        $data.actionMessage = 'A ação foi realizada com sucesso!';
                        $global.loading = false;
                        $data.model = resp;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.executeGetActionAsync = function (action) {
                    var deferred = $q.defer();
                    $global.loading = true;
                    $api(action).getAction().$promise.then(function (resp) {
                        $global.loading = false;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });
                    return deferred.promise;
                };

                this.executeActionAsync = function (action, entity) {
                    var deferred = $q.defer();
                    $global.loading = true;
                    $api(action).insert(entity).$promise.then(function (resp) {
                        $global.loading = false;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);                
                    });
                    return deferred.promise;
                };

                this.deleteEntity = function($scope, controllerName, model, action, properties) {
                    var self = this;
                    var deleteUser = $window.confirm('Você tem certeza que deseja excluir este registro?');
                    if (deleteUser) {
                        $global.loading = true;
            
                        $api(controllerName).remove({ id: model.id }).$promise.then(function (response) {
                            $global.loading = false;
                            self.onChanged($scope, action, properties);
                        }, function (errResponse) {
                            $global.loading = false;
                            $error.show(errResponse);
                        });
                    }
                };

                this.deleteEntityById = function ($scope, controllerName, id, action, properties) {
                    var self = this;
                    var deleteUser = $window.confirm('Você tem certeza que deseja excluir este registro?');
                    if (deleteUser) {
                        $global.loading = true;

                        $api(controllerName).remove({ id: id }).$promise.then(function (response) {
                            self.onChanged($scope, action, properties);
                        }, function (errResponse) {
                            $error.show(errResponse);
                        });
                    }
                };

                this.deleteEntityAsync = function (controllerName, model) {
                    var deleteUser = $window.confirm('Você tem certeza que deseja excluir este registro?');
                    if (deleteUser) {
                        var deferred = $q.defer();
                        $global.loading = true;

                        $api(controllerName).remove({ id: model.id }).$promise.then(function (response) {
                            $global.loading = false;
                            deferred.resolve(response);
                        }, function (errResponse) {
                            $global.loading = false;
                            deferred.reject(errResponse);
                        });

                        return deferred.promise;
                    }
                };

                this.deleteEntityByIdAsync = function (controllerName, id) {
                    var deleteUser = $window.confirm('Você tem certeza que deseja excluir este registro?');
                    if (deleteUser) {
                        var deferred = $q.defer();
                        $global.loading = true;

                        $api(controllerName).remove({ id: id }).$promise.then(function (response) {
                            $global.loading = false;
                            deferred.resolve(response);
                        }, function (errResponse) {
                            $global.loading = false;
                            deferred.reject(errResponse);
                        });

                        return deferred.promise;
                    }
                };

                this.deleteEntityFromList = function (propertyName, listName, entity) {
                    var index = $data.entity[propertyName][listName].indexOf(entity);
                    $data.entity[propertyName][listName].splice(index, 1);
                };

                this.deleteFileEntity = function ($scope, controllerName, entity, url, action, properties) {
                    var self = this;

                    var deleteUser = $window.confirm('Você tem certeza que deseja excluir este registro?');
                    if (deleteUser) {
                        $global.loading = true;
                        $http.post('/home/removefile', { url: url })
                            .success(function (data, status, headers, config) {
                                $api(controllerName).remove({ id: entity.id }).$promise.then(function (resp) {
                                    self.onChanged($scope, action, properties);
                                }, function (errResponse) {
                                    error.show('Erro', 'OK', errResponse.data);
                                    $global.loading = false;
                                });
                            })
                            .error(function (responseData) {
                                $global.loading = false;
                                $error.show(errResponse, 'Não foi possível excluir o arquivo ' + entity.url);
                                $global.loading = false;
                            });
                    }
                };

                this.getEntities = function ($scope, controllerName, propertyName, parent, currentPage, pageSize, filtersType, fields, operators, values, orderFields, directions) {
                    $global.loading = true;

                    $api(controllerName).getAll({ parentKey: parent.key, parentValue: parent.value, page: currentPage, take: pageSize, type: 'PagedGrid', hasDefaultConditions: $scope.hasDefaultConditions, filtersType: filtersType, fields: fields, operators: operators, values: values, ordersBy: orderFields, directions: directions }).$promise.then(function (response) {
                        $timeout(function() {
                            $data.entity[propertyName] = response;
                            $global.loading = false;
                        });
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getEntitiesAsync = function (controllerName, propertyName, parent, currentPage, pageSize, filtersType, fields, operators, values, orderFields, directions) {
                    var deferred = $q.defer();
                    $global.loading = true;

                    $api(controllerName).getAll({ parentKey: parent.key, parentValue: parent.value, page: currentPage, take: pageSize, type: 'PagedGrid', hasDefaultConditions: $scope.hasDefaultConditions, filtersType: filtersType, fields: fields, operators: operators, values: values, ordersBy: orderFields, directions: directions }).$promise.then(function (response) {
                        $data.entity[propertyName] = response;
                        $global.loading = false;
                        deferred.resolve(response);                
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                        $global.loading = false;
                    });
                };

                this.reloadData = function ($scope, propertyName) {
                    $global.loading = true;
                    $global.loadingData = true;
                    $data.parseFilterConditions(propertyName);
                    this.getEntities($scope, $data.entity[propertyName].controllerName, propertyName, $data.entity[propertyName].parentId, $data.entity[propertyName].pagingOptions.pageIndex, $data.entity[propertyName].enablePaging ? $data.entity[propertyName].pagingOptions.pageSize : 999, $data.entity[propertyName].filtersType, $data.entity[propertyName].fields, $data.entity[propertyName].operators, $data.entity[propertyName].values, $data.entity[propertyName].sortOptions.fields, $data.entity[propertyName].sortOptions.directions);
                };

                this.getData = function ($scope, id, action) {
                    var self = this;
                    $global.loading = true;
            
                    $api($scope.entityNameUpdate).get({ id: id }).$promise.then(function (resp) {
                        $data.entity = resp;
                        $data.condition = resp.angularConditions;

                        $data.listen($scope);
                        self.listen($scope);

                        if (action === 'update' && typeof onUpdated === 'function')
                                onUpdated($scope, $data.entity);

                        $global.loading = false;
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getDataAsync = function ($scope, id, action) {
                    var deferred = $q.defer();
                    var self = this;
                    $global.loading = true;
            
                    $api($scope.entityNameUpdate).get({ id: id }).$promise.then(function (resp) {
                        $data.entity = resp;
                        $data.condition = resp.angularConditions;

                        $data.listen($scope);
                        self.listen($scope);

                        if (action === 'update' && typeof onUpdated === 'function')
                            onUpdated($scope, $data.entity);

                        $global.loading = false;
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                        $global.loading = false;
                    });

                    return deferred.promise;
                };

                this.getDataWithParametersAsync = function (controllerName, parameters) {
                    var deferred = $q.defer();
                    var self = this;
                    $global.loading = true;

                    $api(controllerName).get(parameters).$promise.then(function (resp) {
                        deferred.resolve(resp);
                    }, function (errResponse) {
                        deferred.reject(errResponse);
                    });

                    return deferred.promise;
                };

                this.getCepAsync = function(cep) {
                    var deferred = $q.defer();
                    $global.loading = true;
            
                    $api('cepservice').get({ cep: cep }).$promise.then(function (response) {
                        $global.loading = false;
                        deferred.resolve(response);
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);                
                    });

                    return deferred.promise;
                };

                this.listen = function($scope) {
                    var self = this;
                    if ($scope.condition !== null) {
                        for (var prop in $scope.condition) {
                            $scope.$watch('data.entity.' + prop + '.pagingOptions', function (newVal, oldVal) {
                                var propertyName = this.exp.replace('data.entity.', '').replace('.pagingOptions', '');
                                if (newVal !== oldVal && (newVal.pageSize !== oldVal.pageSize)) {
                                    self.reloadData(propertyName);
                                }
                                $scope.loadingData = false;
                            }, true);
                        }
                    }
                };

                this.listenList = function($scope) {
                    var self = this;
                    if ($datalist.condition !== null) {
                        $scope.$watch('data.entity.pagingOptions', function (newVal, oldVal) {
                            if (newVal !== oldVal && (newVal.pageSize !== oldVal.pageSize)) {
                                self.reloadDataList($scope);
                            }
                            $global.loadingData = false;
                        }, true);                        
                    }
                };

                this.reloadDataList = function ($scope, params) {
                    $global.loading = true;
                    $global.loadingData = true;
                    
                    if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params) && typeof params.page !== 'undefined' &&  typeof params.take !== 'undefined') {
                        $datalist.entity.pagingOptions.pageIndex = params.page;
                        $datalist.entity.pagingOptions.pageSize = params.take;
                        if (typeof params.parentKey !== 'undefined')
                            $datalist.parentKey = params.parentKey;
                        if (typeof params.parentValue !== 'undefined')
                            $datalist.parentValue = params.parentValue;
                    }
                    
                    var pageIndex = (typeof $datalist.entity.pagingOptions === 'undefined') ? $datalist.pagingOptions.pageIndex : $datalist.entity.pagingOptions.pageIndex;
                    var pageSize = (typeof $datalist.entity.pagingOptions === 'undefined') ? $datalist.pagingOptions.pageSize : $datalist.entity.pagingOptions.pageSize;
                    var orderFields = (typeof $datalist.entity.sortOptions === 'undefined' || $datalist.entity.sortOptions == null) ? $datalist.sortOptions.fields : $datalist.entity.sortOptions.fields;
                    var orderDirections = (typeof $datalist.entity.sortOptions === 'undefined' || $datalist.entity.sortOptions == null) ? $datalist.sortOptions.directions : $datalist.entity.sortOptions.directions;
                    
                    var parentKey = '';
                    var parentValue = '';
                    if (typeof $datalist.parentKey !== 'undefined' && typeof $datalist.parentValue !== 'undefined') {
                        parentKey = $datalist.parentKey;
                        parentValue = $datalist.parentValue;
                    }

                    this.getEntitiesList($scope.entityName, pageIndex, pageSize, $datalist.entity.filtersType, $datalist.entity.fields, $datalist.entity.operators, $datalist.entity.values, orderFields, orderDirections, parentKey, parentValue);
                };

                this.reloadDataListAsync = function ($scope, params) {
                    var deferred = $q.defer();
                    $global.loading = true;
                    $global.loadingData = true;

                    if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params)) {
                        $datalist.entity.pagingOptions.pageIndex = params.page;
                        $datalist.entity.pagingOptions.pageSize = params.take;
                        if (typeof params.parentKey !== 'undefined')
                            $datalist.parentKey = params.parentKey;
                        if (typeof params.parentValue !== 'undefined')
                            $datalist.parentValue = params.parentValue;
                    }

                    var pageIndex = (typeof $datalist.entity.pagingOptions === 'undefined') ? $datalist.pagingOptions.pageIndex : $datalist.entity.pagingOptions.pageIndex;
                    var pageSize = (typeof $datalist.entity.pagingOptions === 'undefined') ? $datalist.pagingOptions.pageSize : $datalist.entity.pagingOptions.pageSize;
                    var orderFields = (typeof $datalist.entity.sortOptions === 'undefined' || $datalist.entity.sortOptions == null) ? $datalist.sortOptions.fields : $datalist.entity.sortOptions.fields;
                    var orderDirections = (typeof $datalist.entity.sortOptions === 'undefined' || $datalist.entity.sortOptions == null) ? $datalist.sortOptions.directions : $datalist.entity.sortOptions.directions;
                    
                    var parentKey = '';
                    var parentValue = '';
                    if (typeof $datalist.parentKey !== 'undefined' && typeof $datalist.parentValue !== 'undefined') {
                        parentKey = $datalist.parentKey;
                        parentValue = $datalist.parentValue;
                    }

                    var promise = this.getEntitiesListAsync($scope.entityName, pageIndex, pageSize, $datalist.entity.filtersType, $datalist.entity.fields, $datalist.entity.operators, $datalist.entity.values, orderFields, orderDirections, parentKey, parentValue);
                    promise.then(function (response) {
                        deferred.resolve(response);
                    }, function (reason) {
                        deferred.reject(reason);
                    });

                    return deferred.promise;
                };
                
                this.getEntitiesList = function (controllerName, currentPage, pageSize, filtersType, fields, operators, values, orderFields, directions, parentKey, parentValue) {
                    $global.loading = true;
                    var params = { page: currentPage, take: pageSize, type: 'PagedGrid', hasDefaultConditions: $datalist.entity.hasDefaultConditions, filtersType: filtersType, fields: fields, operators: operators, values: values, ordersBy: orderFields, directions: directions, parentKey: parentKey, parentValue: parentValue };
                    $api(controllerName).getAll(params).$promise.then(function (response) {
                        $timeout(function() {
                            $datalist.entity = response;
                            $datalist.condition = response.condition;

                            if ($global.enableQueryString && !$datalist.firstExecution)
                                $location.search(params);

                            $timeout(function () { $datalist.firstExecution = false; }, 250);
                            $global.loading = false;
                        });
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };

                this.getEntitiesListAsync = function (controllerName, currentPage, pageSize, filtersType, fields, operators, values, orderFields, directions, parentKey, parentValue) {
                    var deferred = $q.defer();
                    $global.loading = true;

                    $api(controllerName).getAll({ page: currentPage, take: pageSize, type: 'PagedGrid', hasDefaultConditions: $datalist.entity.hasDefaultConditions, filtersType: filtersType, fields: fields, operators: operators, values: values, ordersBy: orderFields, directions: directions, parentKey: parentKey, parentValue: parentValue }).$promise.then(function (response) {
                        $timeout(function () {
                            $datalist.entity = response;
                            $datalist.condition = response.condition;
                            $global.loading = false;
                            deferred.resolve(response);
                        });
                    }, function (errResponse) {
                        $global.loading = false;
                        deferred.reject(errResponse);
                    });

                    return deferred.promise;
                };

                this.reloadModalDataList = function ($scope, params) {
                    $global.loading = true;
                    $global.loadingData = true;

                    if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params)) {
                        $datalist.modalEntity.pagingOptions.pageIndex = params.page;
                        $datalist.modalEntity.pagingOptions.pageSize = params.take;
                        if (typeof params.parentKey !== 'undefined')
                            $datalist.parentKey = params.parentKey;
                        if (typeof params.parentValue !== 'undefined')
                            $datalist.parentValue = params.parentValue;
                    }
                    
                    var pageIndex = (typeof $datalist.modalEntity.pagingOptions === 'undefined') ? $datalist.modalPagingOptions.pageIndex : $datalist.modalEntity.pagingOptions.pageIndex;
                    var pageSize = (typeof $datalist.modalEntity.pagingOptions === 'undefined') ? $datalist.modalPagingOptions.pageSize : $datalist.modalEntity.pagingOptions.pageSize;
                    var orderFields = (typeof $datalist.modalEntity.sortOptions === 'undefined' || $datalist.modalEntity.sortOptions == null) ? $datalist.modalSortOptions.fields : $datalist.modalEntity.sortOptions.fields;
                    var orderDirections = (typeof $datalist.modalEntity.sortOptions === 'undefined' || $datalist.modalEntity.sortOptions == null) ? $datalist.modalSortOptions.directions : $datalist.modalEntity.sortOptions.directions;
                    console.log($scope.entityName);

                    var parentKey = '';
                    var parentValue = '';
                    if (typeof $datalist.modalParentKey !== 'undefined' && typeof $datalist.modalParentValue !== 'undefined') {
                        parentKey = $datalist.modalParentKey;
                        parentValue = $datalist.modalParentValue;
                    }

                    this.getModalEntitiesList($scope.entityName, pageIndex, pageSize, $datalist.modalEntity.filtersType, $datalist.modalEntity.fields, $datalist.modalEntity.operators, $datalist.modalEntity.values, orderFields, orderDirections, parentKey, parentValue);
                };

                this.getModalEntitiesList = function (controllerName, currentPage, pageSize, filtersType, fields, operators, values, orderFields, directions, parentKey, parentValue) {
                    $global.loading = true;

                    var params = { page: currentPage, take: pageSize, type: 'PagedGrid', hasDefaultConditions: $datalist.modalEntity.hasDefaultConditions, filtersType: filtersType, fields: fields, operators: operators, values: values, ordersBy: orderFields, directions: directions, parentKey: parentKey, parentValue: parentValue };
                    $api(controllerName).getAll(params).$promise.then(function (response) {
                        $timeout(function () {
                            $datalist.modalEntity = response;
                            $datalist.modalCondition = response.condition;

                            if ($global.enableQueryString && !$datalist.firstModalExecution)
                                $location.search(params);

                            $timeout(function () { $datalist.firstExecution = false; }, 250);
                            $global.loading = false;
                        });
                    }, function (errResponse) {
                        $global.loading = false;
                        $error.show(errResponse);
                    });
                };
        } ]);

})();