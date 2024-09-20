import { Component, OnInit, inject, Input, Output, EventEmitter } from '@angular/core';
import { TranslatorService } from '../../core/services/translator.service';
import { Translator } from '../../core/models/translator.model';
import { CommonModule } from '@angular/common';
import { StatusPipe } from '../../utils/status-pipe';
import { ActivatedRoute } from '@angular/router';


@Component({
    standalone: true,
    imports: [CommonModule, StatusPipe],
    selector: 'app-translators',
    templateUrl: './translators.component.html'
})
export class TranslatorsComponent implements OnInit {
    @Input() translators!: Translator[];
    @Input() isVisible = false; // Track visibility
    @Output() translatorIdSent = new EventEmitter<string>();
    @Input() canChoose = true;
    translatorService = inject(TranslatorService);
    route = inject(ActivatedRoute);

    ngOnInit(): void {
        this.route.data.subscribe(data => {
            this.isVisible = data['isVisible'];
            this.canChoose = data['canChoose'] ?? true;
        });
        this.translatorService.getTranslators().subscribe(data => {
            this.translators = data;
            console.log(this.translators)
        });
    }

    updateTrasnlator(translotorId: string): void {
        this.translatorIdSent.emit(translotorId)
    }

}
