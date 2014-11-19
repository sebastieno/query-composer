/// <reference path="./typings/knockout/knockout.d.ts" />
/// <reference path="./typings/knockout.viewmodel/knockout.viewmodel.d.ts" />

module QueryComposer {
    export module Model {
        /*
         * Type of the field
         */
        export enum FieldTypes {
            Text,
            List,
            Multiple
        }

        export interface IQuery {
        }

        /*
         * Model representing a query
         */
        export class Query implements IQuery {
            /*
             * Field used for the query
             */
            public field: KnockoutObservable<FieldDefinition> = ko.observable(null);

            /*
             * Value used for the query
             */
            public value: KnockoutObservable<string> = ko.observable("");

            /*
             * Operator between this query and the previous one
             */
            public operator: KnockoutObservable<string> = ko.observable("");
        }

        /**
         * Represents a list value (used for the list field definition)
         */
        export class ListValue {
            public value: number;
            public text: string;

            constructor(value: number, text: string) {
                this.value = value;
                this.text = text;
            }
        }

        /*
         * Interface representing a field definition
         */
        export interface FieldDefinition {
            name: string;
            text: string;
            type: FieldTypes;
        }

        /*
         * Model representing a text field definition
         */
        export class TextFieldDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public type: FieldTypes = FieldTypes.Text;

            constructor(name: string, text: string) {
                this.name = name;
                this.text = text;
            }
        }

        /*
         * Model representing a list field definition
         */
        export class ListFieldDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public values: ListValue[];
            public type: FieldTypes = FieldTypes.List;

            constructor(name: string, text: string, values: ListValue[]) {
                this.name = name;
                this.text = text;
                this.values = values;
            }
        }

        /*
         * Model representing a field composed of a main field, and some childs fields
         */
        export class MultipleFieldsDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public mainField: FieldDefinition;
            public type: FieldTypes = FieldTypes.Multiple;

            public childrenFields: FieldDefinition[];

            //public subQueries: Query[] = [];

            constructor(mainField: FieldDefinition, childrenFields: FieldDefinition[]) {
                this.mainField = mainField;
                this.name = mainField.name;
                this.text = mainField.text;
                this.childrenFields = childrenFields;

                //childrenFields.forEach(field => {
                //    var query = new Query();
                //    query.field(field);

                //    this.subQueries.push(query);
                //});
            }
        }
    }

    export class Configuration {
        public showNewEmptyLine: boolean;
    }

    /*
     * View model used to create queries composition
     */
    export class QueriesViewModel {
        /*
         * Queries of the composition
         */
        public queries: KnockoutObservableArray<Model.IQuery> = ko.observableArray([]);

        /*
         * List of fields definition
         */
        public fieldsDefinition: Model.FieldDefinition[];

        /*
         * Operators list, used between each query
         */
        public operators: any = [{ name: 'ET', value: '&&' }, { name: 'OU', value: '||' }];

        constructor(fieldsDefinition: Model.FieldDefinition[], configuration: Configuration, queries: any) {
            this.fieldsDefinition = fieldsDefinition;

            if (queries) {
                for (var i = 0; i < queries.length; i++) {
                    var query = new Model.Query();

                    var fields = this.fieldsDefinition.filter(function (field) {
                        return true; // TODO: field.name == queries[i].field;
                    });

                    if (fields.length === 1) {
                        query.field(fields[0]);

                        if (this.queries().length == 0) {
                            query.operator("&&");
                        } else {
                            query.operator(queries[i].operator);
                        }

                        query.value(queries[i].value);

                        this.queries.push(query);
                    } else {
                        console.warn("The field " + queries[i].field + " cannot be retrieved from the fields definition");
                    }
                }
            }

            if (configuration && configuration.showNewEmptyLine) {
                this.addQuery();
            }
        }

        addQuery(): void {
            this.queries.push({});
        }

        removeQuery(query: Model.Query): void {
            this.queries.remove(query);
        }

        goUp(query: Model.Query): void {
            var currentIndex = this.queries.indexOf(query);
            var targetIndex = currentIndex - 1;

            if (targetIndex < 0) return;

            var currentElement = this.queries()[targetIndex];

            this.queries.valueWillMutate();
            this.queries()[currentIndex] = currentElement;
            this.queries()[targetIndex] = query;
            this.queries.valueHasMutated();
        }

        goDown(query: Model.Query): void {
            var currentIndex = this.queries.indexOf(query);
            var targetIndex = currentIndex + 1;

            if (targetIndex >= this.queries().length) return;

            var currentElement = this.queries()[targetIndex];

            this.queries.valueWillMutate();
            this.queries()[currentIndex] = currentElement;
            this.queries()[targetIndex] = query;
            this.queries.valueHasMutated();
        }
    }
}