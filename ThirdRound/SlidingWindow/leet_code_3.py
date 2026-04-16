# longest substring without repeating characters

def lenghtOfLongestSubstring(s: str) -> int: 
    l = 0
    sett = set()
    longest = 0
    
    for r in range(len(s)):
        while s[r] in sett: 
            sett.remove(s[l])
            l += 1
            
        w = (r - l) + 1
        longest = max(longest, w)
        sett.add(s[r])
        
    return longest

print(lenghtOfLongestSubstring("abcabcbb")) 
    