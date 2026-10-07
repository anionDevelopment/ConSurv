import { environment } from "../../environments/environment";
import packageInfo from '../../../package.json';

export class Settings {
    public static getAPIUrl(): string {
        return window.location.origin;
    }
    public static isVerbose(): boolean {
        return environment.development;
    }
    public static isDevelopment(): boolean {
        return environment.development;
    }
    public static isProduction(): boolean {
        return environment.production;
    }
    public static getAppName(): string {
        /*
         * The name of the product and not the name of the npm-package: the package-name has to be lowercase
         * and without spaces (npm requires that), so using it here displayed "con-surv-frontend" as the title
         * of every page and in the footer. The version below is taken from the package, because that value is
         * the version of the product.
         */
        return "ConSurv";
    }
    public static getAppVersion(): string {
        return packageInfo.version;
    }
}
