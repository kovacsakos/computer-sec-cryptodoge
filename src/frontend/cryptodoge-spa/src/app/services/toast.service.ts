import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor(private messageService: MessageService) { }

  success(msg: string, details?: string) {
    this.messageService.add({severity: "success", summary: msg, detail: details});
  }

  error(msg: string, details?: string) {
    this.messageService.add({severity: "error", summary: msg, detail: details});
  }

  info(msg: string, details?: string) {
    this.messageService.add({severity: "info", summary: msg, detail: details});
  }

  warning(msg: string, details?: string) {
    this.messageService.add({severity: "warn", summary: msg, detail: details});
  }
}