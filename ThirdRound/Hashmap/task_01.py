# keys in hashmaps must IMMUTABLE 
# keys are unique and can't be reassigned 


# Initialization 
from collections import defaultdict
from typing import List


hash_map = {} 
hash_map = dict()

# Adding data 
city_map = {}
cities = ["calgary", "vancouver", "toronto"]

city_map["Canada"] = []
city_map["Canada"] += cities

# Or by using default dict 
city_map_defaultdict = defaultdict(list)
city_map_defaultdict["Canada"] += cities

# Retrieving Data 
hash_map.keys() 
hash_map.values()
hash_map.items()



data = {1: 2, 3: 4, 4: 3, 2: 1, 0: 0}
dic1 = {1: 10, 2: 20}
dic2 = {3: 30, 4: 40}

def sort_dict_value(d):
    value = input("1. Acs or 2. Dsc\n")
    if value == "1":
        return dict(sorted(d.items(), key=lambda item: item[1]))
    else: 
        return dict(sorted(d.items(), key=lambda item: item[1], reverse=True))

def sort_dict_key(d):
    value = input("1. Acs or 2. Dsc\n")
    if value == "1":
        return dict(sorted(d.items(), key=lambda item: item[0]))
    else:
        return dict(sorted(d.items(), key=lambda item: item[0], reverse=True))

def add_key(d):
    d.update({2: 30})

def remove_key(d):
    if 1 in d:
        del d[1]
        
    return d

def cancate_dict():
    dic1 = {1: 10, 2: 20}
    dic2 = {3: 30, 4: 40}
    dic3 = {5: 50, 6: 60}
    
    dic = {}
    
    for d in (dic1, dic2, dic3):
        dic.update(d)
    
    return dic

def check_key(d):
    key = 7
    
    if key in d.keys():
        return True
    return False

def iterate(d):
    for k, v in d.items():
        print(f"Key: {k} -> Value: {v}")
    
def gen_dic_square(n):
    square_dict = {}
    
    for i in range(1, 6):
        square_dict[i] = i * i 
    
    return square_dict

def merge_dict(d1, d2):
    dic_merge = {}
    for d in (d1, d2):
        dic_merge.update(d)

    return dic_merge

def sum_values(d):
    sum = 0
    for k, v in d.items():
        sum += v
    return sum

def map_list_into_dict(l1, l2):
    combo = []
    
    for l in l1:
        combo.append(l)
    for l in l2:
        combo.append(l)
        
    return dict(combo)
    
def combine_two_dict_by_adding_value(d1, d2):
    result = d1.copy()
    
    for k, v in d2.items():
        result[k] = result.get(k, 0) + v
        
    return result

if __name__ == '__main__':
    d1 = {'a': 100, 'b': 200, 'c':300}
    d2 = {'a': 300, 'b': 200, 'd':400}
    
    print(combine_two_dict_by_adding_value(d1, d2))