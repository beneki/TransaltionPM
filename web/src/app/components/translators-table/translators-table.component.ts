import { Component, OnInit, inject, Input, Output, EventEmitter } from '@angular/core';
import { TranslatorService } from '../../core/services/translator.service';
import { Translator } from '../../core/models/translator.model';
import { CommonModule } from '@angular/common';
import { StatusPipe } from '../../utils/status-pipe';


@Component({
    standalone: true,
    imports: [CommonModule, StatusPipe],
    selector: 'app-translators-table',
    templateUrl: './translators-table.component.html'
})
export class TranslatorsTableComponent implements OnInit {
    @Input() translators!: Translator[];
    @Input() isVisible = false; // Track visibility
    @Output() translatorIdSent = new EventEmitter<string>();
    translatorService = inject(TranslatorService);

    ngOnInit(): void {
        this.translatorService.getTranslators().subscribe(data => {
            this.translators = data;
            console.log(this.translators)
        });
    }

    updateTrasnlator(translotorId: string): void {
        this.translatorIdSent.emit(translotorId)
    }

}
