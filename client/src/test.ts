// Test setup for Angular unit tests (Karma/Jasmine)
import 'zone.js';
import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import { BrowserTestingModule, platformBrowserTesting } from '@angular/platform-browser/testing';
import { provideZonelessChangeDetection } from '@angular/core';

getTestBed().initTestEnvironment(BrowserTestingModule, platformBrowserTesting());

// Configure TestBed with zoneless change detection globally so tests match the
// application's zoneless runtime (prevents NgZone/Zone.js requirement errors).
getTestBed().configureTestingModule({ providers: [provideZonelessChangeDetection()] });
