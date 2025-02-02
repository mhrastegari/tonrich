﻿class App {
    public static setCookie(name: string, value: string, seconds: number) {
        const date = new Date();
        date.setSeconds(date.getSeconds() + seconds);
        document.cookie = `${name}=${value};expires=${date.toUTCString()};path=/`;
    }

    public static getCookie(name: string): string | null {
        // https://stackoverflow.com/a/25490531/2720104
        return document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)')?.pop() || null;
    }

    public static removeCookie(name: string): void {
        document.cookie = `${name}=; Max-Age=0`;
    }

    public static goBack(): void {
        window.history.back();
    }

    public static copy(text: string): void {
        navigator.clipboard.writeText(text).then(function () {
            console.log(text, " copied to clipboard!");
        })
            .catch(function (error) {
                console.error(error);
            });
    }

    public static setLocalStorageItem(key: string, value: string): void {
        localStorage.setItem(key, value);
    }

    public static getLocalStorageItem(key: string): string | null {
        return localStorage.getItem(key);
    }
}
