/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class UserService {
    /**
     * @param user
     * @param password
     * @returns string OK
     * @throws ApiError
     */
    public static putApiV0UserControllerCreateUser(
        user?: string,
        password?: string,
    ): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/API/v0/UserController/CreateUser',
            headers: {
                'user': user,
                'password': password,
            },
        });
    }
    /**
     * @param user
     * @param password
     * @returns string OK
     * @throws ApiError
     */
    public static putApiV0UserControllerLogin(
        user?: string,
        password?: string,
    ): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/API/v0/UserController/Login',
            headers: {
                'user': user,
                'password': password,
            },
        });
    }
    /**
     * @param token
     * @returns any OK
     * @throws ApiError
     */
    public static putApiV0UserControllerLogout(
        token?: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/API/v0/UserController/Logout',
            headers: {
                'token': token,
            },
        });
    }
}
