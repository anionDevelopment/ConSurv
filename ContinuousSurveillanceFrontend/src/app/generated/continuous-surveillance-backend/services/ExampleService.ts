/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ExampleService {
    /**
     * @param _function
     * @param parameter1
     * @param parameter2
     * @returns number OK
     * @throws ApiError
     */
    public static getApiV0ExampleControllerCalculator(
        _function: string,
        parameter1?: number,
        parameter2?: number,
    ): CancelablePromise<number> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/v0/ExampleController/Calculator/{function}',
            path: {
                'function': _function,
            },
            query: {
                'parameter1': parameter1,
                'parameter2': parameter2,
            },
        });
    }
}
