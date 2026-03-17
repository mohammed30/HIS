import json
from collections import Counter

file_path = r'C:\Users\Mohammed\source\repos\mohammed30\HIS\src\src\HIS.Domain.Shared\Localization\HIS\ar.json'

with open(file_path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

keys = []
for line in lines:
    line = line.strip()
    if line.startswith('"') and ':' in line:
        key = line.split('"')[1]
        keys.append(key)

duplicates = [item for item, count in Counter(keys).items() if count > 1]
print("Duplicate keys found:")
for dup in duplicates:
    print(dup)
