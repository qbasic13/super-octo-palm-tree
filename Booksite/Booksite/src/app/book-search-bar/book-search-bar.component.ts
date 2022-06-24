import { Component, Injectable, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime, tap, switchMap, finalize } from 'rxjs/operators';
import { BookSearchService } from 'src/app/services/book-search.service';
import { BookSearchResult } from 'src/app/models/book-search.model';
import { Router } from '@angular/router';
import { MatAutocompleteActivatedEvent, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-book-search-bar',
  templateUrl: './book-search-bar.component.html',
  styleUrls: ['./book-search-bar.component.css']
})
export class BookSearchBarComponent implements OnInit {
  searchBarCtrl = new FormControl();
  bookSearchResults: any;
  isLoading = false;

  constructor(private searchService: BookSearchService,
    private router: Router) { }

  ngOnInit() {
    this.searchBarCtrl.valueChanges
      .pipe(
        debounceTime(500),
        switchMap(value => this.searchService.search(value)
          .pipe(
            finalize(() => {
              this.isLoading = false
            }),
          )
        )
      ).subscribe(searchResults => {
        if (searchResults == undefined) {
          this.bookSearchResults = [];
        } else {
          this.bookSearchResults = searchResults;
        }
      });
  }

  openBookDetails(event: MatAutocompleteSelectedEvent) {
    const selectedIsbn = event.option.value;
    this.searchBarCtrl.setValue('');
    this.router.navigateByUrl(`/book/${selectedIsbn}`);
  }
}
