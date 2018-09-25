/// <reference path="typings\jquery\jquery.d.ts" />
module Test {
    export interface SignalR {
        testHub: ITestHubProxy;
    }
    export interface ITestHubProxy {
        client: ITestHubClient;
        server: ITestHub;
    }
    export interface ITestHubClient {
        updateResultAsync(testResult: TestResult);
    }

    export interface ITestHub {
        startTestAsync(name: string): string;
    }

    export class TestResult {
        private _description: string;
        private _elapsedTime: Date;
        private _exception: string;
        private _name: string;
        private _resultType: string;
        private _runId: string;

        public get description(): string {
            return this._description;
        }

        public set description(value: string) {
            this._description = value;
        }

        public get elapsedTime(): Date {
            return this._elapsedTime;
        }

        public set elapsedTime(value: Date) {
            this._elapsedTime = value;
        }

        public get exception(): string {
            return this._exception;
        }

        public set exception(value: string) {
            this._exception = value;
        }

        public get name(): string {
            return this._name;
        }

        public set name(value: string) {
            this._name = value;
        }

        public get resultType(): string {
            return this._resultType;
        }

        public set resultType(value: string) {
            this._resultType = value;
        }

        public get runId(): string {
            return this._runId;
        }

        public set runId(value: string) {
            this._runId = value;
        }
    }

    export class TestHub {
        public startTestAsync(name: string) {
            
        }
    }

    export class TestHubClient implements ITestHubClient {
        private _htmlDocument;
        constructor(document: HTMLDocument){
        
        }

        public get htmlDocument(): HTMLDocument {
            return this._htmlDocument;
        }

        public set htmlDocument(value: HTMLDocument) {
            this._htmlDocument = value;
        }

        updateResultAsync(testResult: TestResult) {
            console.log('received result ' + testResult.runId);
            // Add the message to the page.
            $('#results').append('<li>' + testResult.name + '</li>');
            
        }
    }
}