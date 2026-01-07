#!/usr/bin/env node
const fs = require('fs');
const path = require('path');

function usage() {
  console.error('Usage: node check-chunks.js <stats.json> <expected-name> [expected-name ...]');
  process.exit(2);
}

const args = process.argv.slice(2);
if (args.length < 1) usage();

const statsPath = path.resolve(process.cwd(), args[0]);
const expected = args.length > 1 ? args.slice(1) : ['agent', 'admin'];

if (!fs.existsSync(statsPath)) {
  console.error(`stats.json not found: ${statsPath}`);
  process.exit(2);
}

let stats;
try {
  stats = JSON.parse(fs.readFileSync(statsPath, 'utf8'));
} catch (err) {
  console.error('Failed to parse stats.json:', err.message);
  process.exit(2);
}

function collectNames(s) {
  const names = new Set();
  if (Array.isArray(s.chunks)) {
    s.chunks.forEach(c => {
      if (Array.isArray(c.names)) c.names.forEach(n => names.add(n));
      if (typeof c.name === 'string') names.add(c.name);
    });
  }
  if (Array.isArray(s.assets)) {
    s.assets.forEach(a => a.name && names.add(a.name));
  }
  if (Array.isArray(s.modules)) {
    s.modules.forEach(m => m.name && names.add(m.name));
  }
  return Array.from(names).map(String).map(n => n.toLowerCase());
}

let names = collectNames(stats);

let missing = expected.filter(e => !names.some(n => n.includes(e.toLowerCase())));

// Fallback: search the raw JSON text for expected substrings (helps different stats shapes)
if (missing.length > 0) {
  const raw = JSON.stringify(stats).toLowerCase();
  missing = missing.filter(e => !raw.includes(e.toLowerCase()));
}

if (missing.length === 0) {
  console.log('OK: found expected lazy chunk names (or substrings):', expected.join(', '));
  process.exit(0);
} else {
  if (names.length === 0) {
    console.error('Missing expected chunk names:', missing.join(', '));
    console.error('No explicit chunk/module names discovered via primary fields.');
    console.error('Tip: run `ng build --stats-json` and inspect the produced stats.json to see available chunk names.');
  } else {
    console.error('Missing expected chunk names:', missing.join(', '));
    console.error('Discovered chunk/module names (sample):', names.slice(0, 50).join(', '));
  }
  process.exit(3);
}
