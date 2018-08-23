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
    
    angular.module('AstoveApp').service('AstoveData', ['AstoveCommon', 
        function ($global) {
            this.EMAIL_REGEXP = /^[a-z0-9!#$%&'*+/=?^_`{|}~.-]+@[a-z0-9-]+\.([a-z0-9]{2,3})+(\.[a-z0-9-]{2})*$/i;
            $global.loading = true;
            this.condition = {};
            this.entity = {};
            this.model = {};
            this.profile = {};
            this.modalModel = {};
            this.entities = {};
            this.sortOptions = {
                fields: ['id'],
                directions: ['asc']
            };

            this.clearCondition = function (propertyName) {
                this.condition[propertyName] = {
                    index: '',
                    property: '',
                    displayName: '',
                    defaultOperator: '',
                    displayOperator: '',
                    operators: [],
                    defaultValue: '',
                    domainValues: [],
                    displayValue: '',
                    filterType: '',
                    exists: '',
                    mask: '',
                    css: '',
                    width: 0,
                    length: 0
                };
            };

            this.parseFilterConditions = function (propertyName) {
                this.entity[propertyName].hasDefaultConditions = false;

                this.entity[propertyName].filtersType = new Array(this.entity[propertyName].filterConditions.length);
                this.entity[propertyName].fields = new Array(this.entity[propertyName].filterConditions.length);
                this.entity[propertyName].operators = new Array(this.entity[propertyName].filterConditions.length);
                this.entity[propertyName].values = new Array(this.entity[propertyName].filterConditions.length);

                for (var i = 0; i < this.entity[propertyName].filterConditions.length; i++) {
                    this.entity[propertyName].filtersType[i] = this.entity[propertyName].filterConditions[i].filterType;
                    this.entity[propertyName].fields[i] = this.entity[propertyName].filterConditions[i].property;
                    this.entity[propertyName].operators[i] = this.entity[propertyName].filterConditions[i].defaultOperator;
                    this.entity[propertyName].values[i] = this.entity[propertyName].filterConditions[i].defaultValue;
                }
            };

            this.setDisplayOperator = function (propertyName, defaultOperator) {
                var operator = _.findWhere(this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].operators, { key: this.condition[propertyName].defaultOperator });
                this.condition[propertyName].displayOperator = '';
                if (typeof operator !== 'undefined')
                    this.condition[propertyName].displayOperator = operator.value;
            };

            this.setDefaultValue = function (propertyName, defaultValue) {
                if (this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].filterType === 'DateFilter')
                    this.condition[propertyName].displayValue = new Date(defaultValue).toLocaleDateString();
                else if (this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].domainValues !== null) {
                    var domainValue = _.findWhere(this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].domainValues, { key: this.condition[propertyName].defaultValue });
                    this.condition[propertyName].displayValue = '';
                    if (typeof domainValue !== 'undefined')
                        this.condition[propertyName].displayValue = domainValue.value;
                }
                else
                    this.condition[propertyName].displayValue = defaultValue;
            };

            this.onSelectedIndex = function (propertyName, index) {
                this.condition[propertyName].property = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].property;
                this.condition[propertyName].displayName = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].displayName;
                this.condition[propertyName].filterType = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].filterType;
                this.condition[propertyName].defaultOperator = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].defaultOperator;
                this.condition[propertyName].operators = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].operators;
                if (this.condition[propertyName].defaultOperator !== '')
                    this.setDisplayOperator(propertyName, this.condition[propertyName].defaultOperator);
                this.condition[propertyName].defaultValue = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].defaultValue;
                this.condition[propertyName].domainValues = this.entity[propertyName].filterOptions.filters[this.condition[propertyName].index].domainValues;
                if (this.condition[propertyName].defaultValue !== '')
                    this.setDefaultValue(propertyName, this.condition[propertyName].defaultValue);
            };

            this.listen = function ($scope) {
                var self = this;
                if (self.condition !== null) {
                    for (var prop in this.condition) {
                        $scope.$watch('data.condition.' + prop, function (newVal, oldVal) {
                            var propertyName = this.exp.replace('data.condition.', '');
                            if (self.condition[propertyName].index === '') {
                                self.clearCondition(propertyName);
                            }
                            else if (newVal !== oldVal && newVal.index !== oldVal.index) {
                                self.onSelectedIndex(propertyName, newVal.index);
                            }
                            else if (newVal !== oldVal && newVal.defaultOperator !== oldVal.defaultOperator) {
                                self.setDisplayOperator(propertyName, newVal.defaultOperator);
                            }
                            else if (newVal !== oldVal && newVal.defaultValue !== oldVal.defaultValue) {
                                self.setDefaultValue(propertyName, newVal.defaultValue);
                            }
                        }, true);
                    }
                }
            };
        }]);

 })();