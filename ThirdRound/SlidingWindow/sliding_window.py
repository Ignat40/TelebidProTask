# mainly used for problems with substrings, subarrays or some window

# variable lenght window (two pointers)

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


# fixed size window ( e.g. k = 3)

def max_sum_of_subarray(arr, k):
    
    cur_sum = 0
    max_sum = 0
    
    for i in range(k):
        cur_sum += arr[i]
        
    max_sum = cur_sum
    
    for j in range(k, len(arr)):
        cur_sum += arr[j]
        cur_sum -= arr[j - k]
    
        max_sum = max(max_sum, cur_sum)
        
    return max_sum

def smallest_cantains(strs, sub_str): # unfinished 
    
    seen = set(sub_str)
    left = 0 
    longest = 0 
    
    for right in range(len(strs)):
        while left in seen:
            seen.remove(strs[left])
            left += 1
        
        cur = (right - left) + 1
        longest = max(longest + cur)
    
    return longest
    
    

print(smallest_cantains("01212", "012"))


