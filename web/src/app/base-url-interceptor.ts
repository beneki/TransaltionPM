import { HttpInterceptorFn } from "@angular/common/http";
import { environment } from '../environments/environment'

export const BaseUrlInterceptor: HttpInterceptorFn = (req, next) => {
    const apiReq = req.clone({ url: `${environment.apiUrl}${req.url}` });
    return next(apiReq);
}