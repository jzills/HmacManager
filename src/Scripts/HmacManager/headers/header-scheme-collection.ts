import { HeaderScheme } from "./header-scheme.js";

export class HeaderSchemeCollection {
    private readonly schemes: HeaderScheme[] = [];

    add(scheme: HeaderScheme): void {
        this.schemes.push(scheme);
    }

    getAll = () => this.schemes;
}