/// <reference path="./typings/knockout/knockout.d.ts" />
/// <reference path="./typings/knockout.viewmodel/knockout.viewmodel.d.ts" />
var QueryComposer;
(function (QueryComposer) {
    var Model;
    (function (Model) {
        /*
         * Type of the field
         */
        (function (FieldTypes) {
            FieldTypes[FieldTypes["Text"] = 0] = "Text";
            FieldTypes[FieldTypes["List"] = 1] = "List";
            FieldTypes[FieldTypes["Multiple"] = 2] = "Multiple";
        })(Model.FieldTypes || (Model.FieldTypes = {}));
        var FieldTypes = Model.FieldTypes;
        /*
         * Model representing a query
         */
        var Query = (function () {
            function Query() {
                /*
                 * Field used for the query
                 */
                this.field = ko.observable(null);
                /*
                 * Value used for the query
                 */
                this.value = ko.observable("");
                /*
                 * Operator between this query and the previous one
                 */
                this.operator = ko.observable("");
                this.dependantQuery = false;
                this.collapsed = ko.observable(true);
                this.resume = ko.observable("");
            }
            return Query;
        })();
        Model.Query = Query;
        /**
         * Represents a list value (used for the list field definition)
         */
        var ListValue = (function () {
            function ListValue(value, text) {
                this.value = value;
                this.text = text;
            }
            return ListValue;
        })();
        Model.ListValue = ListValue;
        /*
         * Model representing a text field definition
         */
        var TextFieldDefinition = (function () {
            function TextFieldDefinition(name, text) {
                this.type = 0 /* Text */;
                this.name = name;
                this.text = text;
            }
            return TextFieldDefinition;
        })();
        Model.TextFieldDefinition = TextFieldDefinition;
        /*
         * Model representing a list field definition
         */
        var ListFieldDefinition = (function () {
            function ListFieldDefinition(name, text, values) {
                this.type = 1 /* List */;
                this.name = name;
                this.text = text;
                this.values = values;
            }
            return ListFieldDefinition;
        })();
        Model.ListFieldDefinition = ListFieldDefinition;
        /*
         * Model representing a multiple fields definition
         */
        var MultipleFieldsDefinition = (function () {
            function MultipleFieldsDefinition(mainField, childrenFields) {
                this.type = 2 /* Multiple */;
                this.name = mainField.name;
                this.text = mainField.text;
                this.mainField = mainField;
                this.childrenFields = childrenFields;
            }
            return MultipleFieldsDefinition;
        })();
        Model.MultipleFieldsDefinition = MultipleFieldsDefinition;
    })(Model = QueryComposer.Model || (QueryComposer.Model = {}));
    var Configuration = (function () {
        function Configuration() {
        }
        return Configuration;
    })();
    QueryComposer.Configuration = Configuration;
    /*
     * View model used to create queries composition
     */
    var QueriesViewModel = (function () {
        function QueriesViewModel(fieldsDefinition, configuration, queries) {
            var _this = this;
            /*
             * Queries of the composition
             */
            this.queries = ko.observableArray([]);
            /*
             * Operators list, used between each query
             */
            this.operators = [{ name: 'ET', value: '&&' }, { name: 'OU', value: '||' }];
            this.fieldsDefinition = fieldsDefinition;
            if (queries) {
                for (var i = 0; i < queries.length; i++) {
                    var query = new Model.Query();
                    var fields = this.fieldsDefinition.filter(function (field) {
                        return field.name == queries[i].field;
                    });
                    if (fields.length === 1) {
                        query.field(fields[0]);
                        if (this.queries().length == 0) {
                            query.operator("&&");
                        }
                        else {
                            query.operator(queries[i].operator);
                        }
                        query.value(queries[i].value);
                        this.queries.push(query);
                        if (query.field().type == 2 /* Multiple */) {
                            var multipleField = query.field();
                            // Gets the sub queries
                            var subQueries = [];
                            i++;
                            while (queries[i] && queries[i].isDependant) {
                                subQueries.push(queries[i]);
                                i++;
                            }
                            multipleField.childrenFields.forEach(function (childField) {
                                var query = new Model.Query();
                                query.operator("&&");
                                query.field(childField);
                                query.dependantQuery = true;
                                var correspondingSubQueries = subQueries.filter(function (q) {
                                    return query.field().name == q.field;
                                });
                                if (correspondingSubQueries.length == 1) {
                                    query.value(correspondingSubQueries[0].value);
                                }
                                _this.queries.push(query);
                            });
                            this.recomputeResume(query);
                        }
                    }
                    else {
                        console.warn("The field " + queries[i].field + " cannot be retrieved from the fields definition");
                    }
                }
            }
            if (configuration && configuration.showNewEmptyLine) {
                this.addQuery();
            }
        }
        QueriesViewModel.prototype.changeQueryFieldDefinition = function (query) {
            var _this = this;
            var index = this.queries.indexOf(query);
            this.queries.valueWillMutate();
            if (!query.field() || query.field().type != 2 /* Multiple */) {
                while (this.queries()[index + 1] && this.queries()[index + 1].dependantQuery) {
                    this.removeQuery(this.queries()[index + 1]);
                }
            }
            if (query.field() && query.field().type == 2 /* Multiple */) {
                var field = query.field();
                var multipleFieldsDefinition = field;
                this.queries.valueWillMutate();
                var fieldIndex = 1;
                multipleFieldsDefinition.childrenFields.forEach(function (childField) {
                    var query = new Model.Query();
                    query.operator("&&");
                    query.field(childField);
                    query.dependantQuery = true;
                    _this.queries.splice(index + fieldIndex, 0, query);
                    fieldIndex++;
                });
            }
            this.queries.valueHasMutated();
        };
        QueriesViewModel.prototype.addQuery = function () {
            this.queries.push(new Model.Query());
        };
        QueriesViewModel.prototype.removeQuery = function (query) {
            var index = this.queries.indexOf(query);
            this.queries.valueWillMutate();
            if (query.field() && query.field().type == 2 /* Multiple */) {
                while (this.queries()[index + 1] && this.queries()[index + 1].dependantQuery) {
                    this.removeQuery(this.queries()[index + 1]);
                }
            }
            this.queries.remove(query);
            this.queries.valueHasMutated();
        };
        QueriesViewModel.prototype.goUp = function (query) {
            var _this = this;
            this.queries.valueWillMutate();
            var queriesToMove = [];
            var startIndex = this.queries.indexOf(query);
            queriesToMove.push(this.queries.splice(startIndex, 1)[0]);
            if (query.field() && query.field().type == 2 /* Multiple */) {
                while (this.queries()[startIndex] && this.queries()[startIndex].dependantQuery) {
                    queriesToMove.push(this.queries.splice(startIndex, 1)[0]);
                }
            }
            var targetIndex = startIndex - 1;
            while (this.queries()[targetIndex] && this.queries()[targetIndex].dependantQuery) {
                targetIndex--;
            }
            queriesToMove.forEach(function (query) {
                _this.queries.splice(targetIndex, 0, query);
                targetIndex++;
            });
            this.queries.valueHasMutated();
        };
        QueriesViewModel.prototype.goDown = function (query) {
            var _this = this;
            this.queries.valueWillMutate();
            var queriesToMove = [];
            var startIndex = this.queries.indexOf(query);
            queriesToMove.push(this.queries.splice(startIndex, 1)[0]);
            if (query.field() && query.field().type == 2 /* Multiple */) {
                while (this.queries()[startIndex] && this.queries()[startIndex].dependantQuery) {
                    queriesToMove.push(this.queries.splice(startIndex, 1)[0]);
                }
            }
            var targetIndex = startIndex + 1;
            while (this.queries()[targetIndex] && this.queries()[targetIndex].dependantQuery) {
                targetIndex++;
            }
            queriesToMove.forEach(function (query) {
                _this.queries.splice(targetIndex, 0, query);
                targetIndex++;
            });
            this.queries.valueHasMutated();
        };
        QueriesViewModel.prototype.toggleCollapse = function (query) {
            var index = this.queries.indexOf(query);
            query.collapsed(!query.collapsed());
            this.queries.valueWillMutate();
            if (query.field().type == 2 /* Multiple */) {
                while (this.queries()[index + 1] && this.queries()[index + 1].dependantQuery) {
                    this.queries()[index + 1].collapsed(query.collapsed());
                    index = index + 1;
                }
            }
            this.queries.valueHasMutated();
        };
        QueriesViewModel.prototype.recomputeResume = function (query) {
            var index = this.queries.indexOf(query) + 1;
            var resume = [];
            if (query.field().type == 2 /* Multiple */) {
                while (this.queries()[index] && this.queries()[index].dependantQuery) {
                    var queryToResume = this.queries()[index];
                    if (queryToResume.value()) {
                        if (queryToResume.field().type == 1 /* List */) {
                            var untypedlistField = queryToResume.field();
                            var listField = untypedlistField;
                            for (var i = 0; i < listField.values.length; i++) {
                                if (listField.values[i].value == parseInt(queryToResume.value())) {
                                    resume.push(" " + queryToResume.field().text + ": " + listField.values[i].text);
                                    break;
                                }
                            }
                        }
                        else {
                            resume.push(" " + queryToResume.field().text + ": " + queryToResume.value());
                        }
                    }
                    index = index + 1;
                }
            }
            query.resume(resume.join(','));
        };
        return QueriesViewModel;
    })();
    QueryComposer.QueriesViewModel = QueriesViewModel;
})(QueryComposer || (QueryComposer = {}));
//# sourceMappingURL=query-composer.js.map